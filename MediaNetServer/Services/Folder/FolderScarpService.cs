using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emby.Naming.Common;
using Emby.Naming.TV;
using Emby.Naming.Video;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Movies;

namespace MediaNetServer.Services.Folder;

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
    public async Task ScrapeFolderAsync(string rootPath)
    {
        // 电影子目录
        var moviesDir = Path.Combine(rootPath, "Movies");
        if (System.IO.Directory.Exists(moviesDir))
        {
            Console.WriteLine($"Found Movies folder: {moviesDir}");
            await ScrapeMoviesAsync(moviesDir);
        }
        else
        {
            Console.WriteLine($"Movies folder not found under {rootPath}");
        }

        // 电视剧子目录
        var seriesDir = Path.Combine(rootPath, "Series");
        if (System.IO.Directory.Exists(seriesDir))
        {
            Console.WriteLine($"Found Series folder: {seriesDir}");
            await ScrapeSeriesAsync(seriesDir);
        }
        else
        {
            Console.WriteLine($"Series folder not found under {rootPath}");
        }
    }
    
    private bool IsUnixHidden(string path)
    {
        return Path.GetFileName(path).StartsWith(".");
    }

    /// <summary>
    /// 处理电影目录：遍历所有视频文件，解析 Title/Year 并查询 TMDb 元数据
    /// </summary>
    private async Task ScrapeMoviesAsync(string folderPath)
    {
        foreach (var file in System.IO.Directory.GetFiles(folderPath))
        {
            // 仅处理视频文件
            if (!VideoResolver.IsVideoFile(file, _namingOptions))
                continue;
            if(IsUnixHidden(file))
                continue;

            // 解析文件名
            var info = VideoResolver.ResolveFile(path:file, namingOptions:_namingOptions);
            var title = info.Name.Replace('.', ' ').Trim();
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
            if (results.Results.Count == 0)
            {
                Console.WriteLine($"没有找到 { query } ");
                continue;
            }

            var mResult = results.Results.FirstOrDefault();
            int mId = mResult.Id;
            
            var movieResult = await _tmdbClient.GetMovieAsync(mId);
            if (movieResult == null)
            {
                Console.WriteLine($"未找到电影：{title} ({year})");
                continue;
            }
            
            Console.WriteLine($"[Movie] {title} ({year}) → TMDb ID: {mId}");
            Console.WriteLine($"{movieResult.Title}, {movieResult.ReleaseDate?.Year}, {movieResult.Overview}");
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
        var files = System.IO.Directory.GetFiles(folderPath)
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

            // 解析出 season/episode
            EpisodeInfo? epInfo = episodeResolver.Resolve(path: file, isDirectory:false);
            var seasonNumber  = epInfo.SeasonNumber  ?? 1;
            var episodeNumber = epInfo.EpisodeNumber ?? 1;
            var seriesName = epInfo.SeriesName;

            // 搜索剧集列表，并优先用年份过滤
            var searchResults = await _tmdbClient.SearchTvShowAsync(seriesName)
                .ConfigureAwait(false);

            if (searchResults.Results.Count == 0)
            {
                // not found
                Console.WriteLine($"未在 TMDb 找到：{seriesName} ");
                continue;
            }
            var sResults = searchResults.Results.FirstOrDefault();
            int TvId = sResults.Id;
            int showYear = sResults.FirstAirDate?.Year ?? 0;

            // 获取该剧集指定季集的元数据
            var epData = await _tmdbClient.GetTvEpisodeAsync(TvId, seasonNumber, episodeNumber);

            Console.WriteLine(
                $"[Series] {seriesName} ({showYear}) → S{seasonNumber:00}E{episodeNumber:00}: {epData.Name}"
            );
        }
    }
}