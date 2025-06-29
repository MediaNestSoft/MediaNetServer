// 文件：MediaNetServer/Services/SeriesScan.cs
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jellyfin.Data.Entities.Libraries;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Data.media.Services;
using TMDbLib.Objects.General;
using TMDbLib.Objects.TvShows;

namespace MediaNetServer.Services
{
    /// <summary>
    /// 遍历 series 根目录，按剧集→季→集层级刮削并保存到数据库
    /// </summary>
    public class SeriesScan
    {
        private readonly FileScanService      _parser;
        private readonly TMDbMetadataService  _tmdbService;
        private readonly MediaItemService     _repo;
        private readonly SeriesFileScan       _seriesFileScan;

        public SeriesScan(
            FileScanService parser,
            TMDbMetadataService tmdbService,
            MediaItemService repo,
            SeriesFileScan seriesFileScan)
        {
            _parser      = parser;
            _tmdbService = tmdbService;
            _repo        = repo;
            _seriesFileScan = seriesFileScan;
        }

        public async Task ScanAndSaveAsync(string rootFolder)
        {
            // rootFolder 下有多个系列文件夹
            foreach (var seriesDir in Directory.GetDirectories(rootFolder))
            {
                var seriesInfo = _seriesFileScan.ParseSeriesFile(seriesDir);
                var dirName = seriesInfo.Name.Trim();
                
                // 提取中英文系列名称，重用之前的逻辑
                bool hasChinese = Regex.IsMatch(dirName, @"\p{IsCJKUnifiedIdeographs}");
                string? chineseTitle = hasChinese
                    ? Regex.Match(dirName, @"[\p{IsCJKUnifiedIdeographs}0-9：:]+").Value.Trim()
                    : null;
                string? englishTitle = Regex.Match(dirName, @"[\p{IsBasicLatin}\-'\s,：:]+").Value.Trim();
                var seriesQuery = hasChinese && !string.IsNullOrEmpty(chineseTitle)
                    ? chineseTitle
                    : englishTitle;
                if (string.IsNullOrWhiteSpace(seriesQuery)) continue;

                // 获取剧集元数据
                var tv = await _tmdbService.GetTvShowMetadataAsync(seriesQuery, null);
                if (tv == null) continue;

                Console.WriteLine($"找到剧集：{tv.Name} ({tv.FirstAirDate:yyyy})");

                // 保存 Series
                var seriesEntity = await _repo.CreateMediaItemAsync(new MediaItem
                {
                    TMDbId       = tv.Id,
                    Title        = tv.Name,
                    ReleaseDate = tv.FirstAirDate,
                    PosterPath   = tv.PosterPath
                });

                // 遍历每一季
                foreach (var season in tv.Seasons.Where(s => s.SeasonNumber > 0))
                {
                    // 拉取季详情，含所有 Episode
                    var seasonDetail = await _tmdbService.GetSeasonAsync(tv.Id, season.SeasonNumber);
                    // 保存 Season（假设 repo 支持 CreateSeasonAsync）
                    /*
                    var seasonEntity = await _repo.CreateSeasonAsync(new Season
                    {
                        SeriesId     = seriesEntity.Id,
                        SeasonNumber = season.SeasonNumber,
                        Title        = season.Name,
                        AirDate      = season.AirDate,
                        PosterPath   = season.PosterPath
                    });*/

                    // 对应文件系统中的季文件夹
                    var seasonDir = Path.Combine(seriesDir, $"Season {season.SeasonNumber}");
                    if (!Directory.Exists(seasonDir)) continue;

                    // 遍历集文件
                    foreach (var file in Directory.EnumerateFiles(seasonDir, "*.*", SearchOption.TopDirectoryOnly))
                    {
                        var info = _parser.ParseFile(file);
                        if (info == null) continue;
                        // 匹配 EpisodeDetail
                        var epDetail = seasonDetail.Results
                            .FirstOrDefault(e => e.EpisodeNumber == info.EpisodeNumber);
                        if (epDetail == null) continue;

                        Console.WriteLine($"  找到 S{season.SeasonNumber}E{epDetail.EpisodeNumber}: {epDetail.Name}");

                        // 保存 Episode（假设 repo 支持 CreateEpisodeAsync）
                        /*
                        await _repo.CreateEpisodeAsync(new Episode
                        {
                            SeasonId      = seasonEntity.Id,
                            EpisodeNumber = epDetail.EpisodeNumber,
                            Title         = epDetail.Name,
                            AirDate       = epDetail.AirDate,
                            StillPath     = epDetail.StillPath
                        });*/
                    }
                }
            }
        }
    }
}