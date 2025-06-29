using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emby.Naming.Video;
using Emby.Naming.TV;
using Emby.Naming.Common;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

public class FolderScraperService
{
    //private readonly EpisodeResolver _episodeResolver;
    private readonly TMDbClient _tmdbClient;
    private readonly NamingOptions _namingOptions = new NamingOptions();

    public FolderScraperService(TMDbClient tmdbClient)
    {
        _tmdbClient = tmdbClient;
            //_episodeResolver = new EpisodeResolver(_namingOptions);
    }

    /// <summary>
    /// 入口：根据文件夹内容判断类型并分支处理
    /// </summary>
    public async Task ScrapeFolderAsync(string folderPath)
    {
        //if (IsSeriesFolder(folderPath))
        //{
            await ScrapeSeriesAsync(folderPath);
        //}
        //else
        //{
         //   await ScrapeMoviesAsync(folderPath);
        //}
    }
    
    private bool IsUnixHidden(string path)
    {
        return Path.GetFileName(path).StartsWith(".");
    }


    /// <summary>
    /// 判断一个目录是否为电视剧：  
    ///  - 存在名为 Season X 的子目录；  
    ///  - 或顶层文件名包含 S01E01 样式；  
    /// </summary>
    private bool IsSeriesFolder(string folderPath)
    {
        // 子目录命名：Season 01, Specials
        if (Directory.GetDirectories(folderPath)
            .Any(d => Regex.IsMatch(Path.GetFileName(d), @"^(Season|Specials)\s*\d+", RegexOptions.IgnoreCase)))
        {
            return true;
        }
        // 顶层文件名：SxxExx 模式
        return Directory.GetFiles(folderPath)
            .Any(f => Regex.IsMatch(Path.GetFileName(f), @"S\d{1,2}E\d{1,2}", RegexOptions.IgnoreCase));
    }

    /// <summary>
    /// 处理电影目录：遍历所有视频文件，解析 Title/Year 并查询 TMDb 元数据
    /// </summary>
    private async Task ScrapeMoviesAsync(string folderPath)
    {
        foreach (var file in Directory.GetFiles(folderPath))
        {
            // 仅处理视频文件
            if (!VideoResolver.IsVideoFile("file", _namingOptions))
                continue;
            if(IsUnixHidden(file))
                continue;

            // 解析文件名
            var info = VideoResolver.ResolveFile(path:"Cars.3.2017.2160p.BluRay.REMUX.HEVC.DTS-HD.MA.TrueHD.7.1.Atmos-老 K.mkv", namingOptions:_namingOptions);
            var title = info.Name;
            var year  = info.Year;
            var safeYear = year ?? 0;
            
            bool hasChinese = Regex.IsMatch(title, @"\p{IsCJKUnifiedIdeographs}");

            string? chineseTitle = null;
            if (hasChinese)
            {
                var cnMatch = Regex.Match(title, @"[\p{IsCJKUnifiedIdeographs}0-9：:]+");
                if (cnMatch.Success)
                    chineseTitle = cnMatch.Value.Trim();
            }

            var enMatch = Regex.Match(title, @"[\p{IsBasicLatin}\-'\s,：:]+");
            string? englishTitle = enMatch.Success
                ? enMatch.Value.Trim()
                : null;

            var query = hasChinese && !string.IsNullOrEmpty(chineseTitle)
                ? chineseTitle
                : englishTitle;

            // 调用 TMDb 搜索电影
            SearchContainer<SearchMovie> results = 
                await _tmdbClient.SearchMovieAsync(query, year: safeYear);
            var bestMatch = results.Results.FirstOrDefault();
            Console.WriteLine($"[Movie] {title} ({year}) → TMDb ID: {bestMatch?.Id}");
        }
    }

    /// <summary>
    /// 处理电视剧目录：  
    /// 1) 读取顶层文件夹名解析剧名与年份；  
    /// 2) 遍历 Season 目录，解析分集；  
    /// 3) 查询 TMDb 剧集元数据
    /// </summary>
    private async Task ScrapeSeriesAsync(string folderPath)
    {
        var files = Directory.GetFiles(folderPath)
            .Where(f =>
                !IsUnixHidden(f) &&
                VideoResolver.IsVideoFile(path:f, _namingOptions)
            );
        
        foreach (var file in files)
        {
            var options1 = new NamingOptions();
            string path2 = "/series/Squid Game S03E04 1080p HEVC x265-MeGusta.mkv";
            var episodeResolver = new EpisodeResolver(options1);
            var fileName = Path.GetFileNameWithoutExtension(file);

            // 清洗出剧名和年份
            //var clean = VideoResolver.CleanDateTime("Squid Game S03E04 1080p HEVC x265-MeGusta", Options);
            //var showName = clean.Name;
            //var showYear = clean.Year;

            // 解析出 season/episode
            EpisodeInfo? epInfo = episodeResolver.Resolve(path: file, isDirectory:false);
            var seasonNumber  = epInfo.SeasonNumber  ?? 1;
            var episodeNumber = epInfo.EpisodeNumber ?? 1;
            var seriesName = epInfo.SeriesName;

            // 搜索剧集列表，并优先用年份过滤
            

            //if (seriesMatch == null)
            //{
             //   Console.WriteLine($"[Warn] 未在 TMDb 找到：{showName} ({showYear})");
             //   continue;
            //}

            // 获取该剧集指定季集的元数据
           /* var epData = await _tmdbClient.GetTvEpisodeAsync(
                ,
                seasonNumber,
                episodeNumber
            );*/

            //Console.WriteLine(
            //    $"[Series] {showName} ({showYear}) → S{seasonNumber:00}E{episodeNumber:00}: {epData.Name}"
            //);
        }
    }
}