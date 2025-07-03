using System;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emby.Naming.Common;
using Emby.Naming.TV;
using Emby.Naming.Video;
using Jellyfin.Data.Entities.Libraries;
using MediaNetServer.Data.media.Data;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Data.media.Services;
using MediaNetServer.Services.MediaServices;
using Microsoft.Extensions.Logging;
using Movie = TMDbLib.Objects.Movies.Movie;
using Season = MediaNetServer.Data.media.Models.Season;

namespace MediaNetServer.Services.Folder;

public class FolderScraperService
{
    //private readonly EpisodeResolver _episodeResolver;
    private readonly TMDbClient _tmdbClient;
    private readonly NamingOptions _namingOptions = new NamingOptions();
    private readonly MediaItemService _itemService;
    private readonly MediaContext  _context;
    private readonly MediaCastService _castService;
    private readonly ImagesService _imagesService;
    private readonly ImageCacheService _iCacheService;
    private readonly SeriesDetailService _seriesDetailService;
    private readonly MovieDetailService _movieDetailService;
    private readonly SeasonService _seasonService;
    private readonly EpisodesService _episodeService;
    private readonly HistoryService _historyService;

    public FolderScraperService(TMDbClient tmdbClient, MediaItemService itemService,
        MediaContext context, MediaCastService castService, ImagesService imagesService,
        ImageCacheService iCacheService, SeriesDetailService seriesDetailService, MovieDetailService movieDetailService,
        SeasonService seasonService, EpisodesService episodeService, HistoryService historyService)
    {
        _tmdbClient = tmdbClient;
        _itemService = itemService;
        _context = context;
        _castService = castService;
        _imagesService = imagesService;
        _iCacheService = iCacheService;
        _seriesDetailService = seriesDetailService;
        _movieDetailService = movieDetailService;
        _seasonService = seasonService;
        _episodeService = episodeService;
        _historyService = historyService;
       // _episodeResolver = new EpisodeResolver(_namingOptions);
    }
    
    /// <summary>
    /// 获取指定文件的最后写入时间。如果文件不存在或读取失败，就返回 UtcNow。
    /// </summary>
    private DateTime GetFileLastModifiedUtc(string filePath)
    {
        try
        {
            return File.GetLastWriteTimeUtc(filePath);
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 入口：根据文件夹内容判断类型并分支处理
    /// </summary>
    public async Task ScrapeFolderAsync(string rootPath)
    {
        var moviesDir  = Path.Combine(rootPath, "Movies");
        var seriesDir  = Path.Combine(rootPath, "Series");

        if(System.IO.Directory.Exists(moviesDir)) 
            await ScrapeMoviesAsync(moviesDir);
       // if(System.IO.Directory.Exists(seriesDir)) 
        //    await ScrapeSeriesAsync(seriesDir);

        //return (movies, episodes);
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
        var list = new List<Movie>();
        foreach (var file in System.IO.Directory.GetFiles(folderPath))
        {
            // 仅处理视频文件
            if (!VideoResolver.IsVideoFile(file, _namingOptions))
                continue;
            if(IsUnixHidden(file))
                continue;
            
            DateTime lastModified = GetFileLastModifiedUtc(file);

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
            
            //var movieResult = await _tmdbClient.GetMovieAsync(mId);
            //if (mResult == null)
            //    continue;
            
            var extraMethods = MovieMethods.Credits | MovieMethods.Images;
            
            var extra = await _tmdbClient.GetMovieAsync(mId,
                language: "zh-CN",
                extraMethods: extraMethods,
                cancellationToken: CancellationToken.None
                );
            if(extra == null)
                continue;

            var logoImage = await _tmdbClient.GetMovieImagesAsync(mId);
            
            var item = new MediaItem
            {
                TMDbId       = extra.Id,
                Title        = extra.Title,
                Type         = "Movie",
                PosterPath   = extra.PosterPath ?? string.Empty,
                BackdropPath = extra.BackdropPath ?? string.Empty,
                LocalPath    = file,
                Rating       = extra.VoteAverage,
                ReleaseDate  = extra.ReleaseDate ?? DateTime.MinValue,
                Country      = extra.ProductionCountries.FirstOrDefault()?.Iso_3166_1 ?? string.Empty,
                AddTime      = lastModified,
                Language     = extra.OriginalLanguage ?? string.Empty,
                LogoPath     = logoImage.Logos[0].FilePath ?? string.Empty,
                Genre        = extra.Genres.Select(g => g.Name).ToList()
            };
            var same = await _itemService.CreateOrGetMediaItemAsync(item);

            // 插入 MovieDetail
            if (!await _movieDetailService.ExistsAsync(same.MediaId))
            {
                var detail = new MovieDetail
                {
                    MediaId = same.MediaId,
                    Overview = extra.Overview ?? string.Empty,
                    Duration = extra.Runtime ?? 0
                };

                await _movieDetailService.CreateAsync(detail);
            }

            var poster = new Data.media.Models.Images
            {
                tmdbId = extra.Id,
                imageType = "Poster",
                filePath = extra.PosterPath
            };
            await _imagesService.AddAsync(poster);
            var backdrop = new Data.media.Models.Images
            {
                tmdbId = extra.Id,
                imageType = "Backdrop",
                filePath = extra.BackdropPath
            };
            await _imagesService.AddAsync(backdrop);
            var logo = new Data.media.Models.Images
            {
                tmdbId = extra.Id,
                imageType = "Logo",
                filePath = logoImage.Logos[0].FilePath
            };
            await _imagesService.AddAsync(logo);
            
            
            await _iCacheService.CacheImageAsync(extra.PosterPath);
            await _iCacheService.CacheImageAsync(extra.BackdropPath);
            await _iCacheService.CacheImageAsync(logoImage.Logos[0].FilePath);

            var files = new Files
            {
                tmdbId = extra.Id,
                //playhistory = 0,
                filePath = file
            };
            _context.Files.Add(files);

            var casts = extra.Credits.Cast
                .Select(c => new MediaCast
                {
                    tmdbId     = extra.Id,
                    Name       = c.Name,
                    Department = c.Character,
                    PersonUrl  = c.ProfilePath    // 头像路径
                })
                .ToList();
            casts.AddRange(
                extra.Credits.Crew
                    .Select(c => new MediaCast
                    {
                        tmdbId     = extra.Id,
                        Name       = c.Name,
                        Department = c.Job,
                        PersonUrl  = c.ProfilePath
                    })
            );
            
            await _castService.CreateAsync(casts);
        }
        
    }
/*
    /// <summary>
    /// 处理电视剧目录：  
    /// 1) 读取顶层文件夹名解析剧名与年份；  
    /// 2) 遍历 Season 目录，解析分集；  
    /// 3) 查询 TMDb 剧集元数据
    /// </summary>
    private async Task ScrapeSeriesAsync(string folderPath)
    {
        var list = new List<TvEpisode>();
        var files = System.IO.Directory.GetFiles(folderPath)
            .Where(f =>
                !IsUnixHidden(f) &&
                VideoResolver.IsVideoFile(path:f, _namingOptions)
            );
        var options1 = new NamingOptions();
        var episodeResolver = new EpisodeResolver(options1);
        
        foreach (var file in files)
        {
            // 仅处理视频文件
            if (!VideoResolver.IsVideoFile(file, _namingOptions))
                continue;
            if(IsUnixHidden(file))
                continue;
            var info = VideoResolver.ResolveFile(path:file, namingOptions:_namingOptions);
            var title = info.Name.Replace('.', ' ').Trim();
            
            DateTime lastModified = GetFileLastModifiedUtc(file);

            // 解析出 season/episode
            EpisodeInfo? epInfo = episodeResolver.Resolve(path: file, isDirectory:false);
            var seasonNumber  = epInfo.SeasonNumber  ?? 1;
            var episodeNumber = epInfo.EpisodeNumber ?? 1;
            var seriesName = epInfo.SeriesName;

            // 搜索剧集列表，并优先用年份过滤
            var searchResults = await _tmdbClient.SearchTvShowAsync(seriesName)
                .ConfigureAwait(false);

            if (searchResults == null)
            {
                // not found
                Console.WriteLine($"未在 TMDb 找到：{title} ");
                continue;
            }
            var sResults = searchResults.Results.FirstOrDefault();
            
            if (sResults == null)
            {
                Console.WriteLine($"未能从 TMDb 搜索结果中找到剧集：{seriesName}");
                continue;
            }

            int TvId = sResults.Id;
            int showYear = sResults.FirstAirDate?.Year ?? 0;
            
            var series = await _tmdbClient.GetTvShowAsync(TvId, language:"zh-CN", extraMethods:TvShowMethods.Credits);
            if (series == null)
            {
                Console.WriteLine($"未能获取剧集详情：{title} ");
                continue;
            }
            
            var logoImage = await _tmdbClient.GetTvShowImagesAsync(series.Id);

            var item = new MediaItem
            {
                TMDbId = TvId,
                Title = series.Name,
                Type = "Series",
                PosterPath = series.PosterPath ?? string.Empty,
                BackdropPath = series.BackdropPath ?? string.Empty,
                LocalPath = file,
                Rating = series.VoteAverage,
                ReleaseDate = series.FirstAirDate ?? DateTime.MinValue,
                Country = series.ProductionCountries.FirstOrDefault()?.Iso_3166_1 ?? string.Empty,
                AddTime = lastModified,
                Language = series.OriginalLanguage ?? string.Empty,
                Genre = series.Genres.Select(g => g.Name).ToList(),
                LogoPath = logoImage.Logos[0].FilePath ?? string.Empty
            };
            var same = await _itemService.CreateOrGetMediaItemAsync(item);

            if (!await _seriesDetailService.ExistsAsync(same.MediaId))
            {
                var detail = new SeriesDetail
                {
                    mediaId = same.MediaId,
                    firstAirDate = series.FirstAirDate ?? DateTime.MinValue,
                    lastAirDate = series.LastAirDate ?? DateTime.MinValue,
                    numberOfEpisodes = series.NumberOfEpisodes,
                    numberOfSeasons = series.NumberOfSeasons,
                    overview = series.Overview,
                };

                await _seriesDetailService.CreateAsync(detail);
                var poster = new Data.media.Models.Images
                {
                    tmdbId = series.Id,
                    imageType = "Poster",
                    filePath = series.PosterPath
                };
                var backdrop = new Data.media.Models.Images
                {
                    tmdbId = series.Id,
                    imageType = "Backdrop",
                    filePath = series.BackdropPath
                };
                var logo = new Data.media.Models.Images
                {
                    tmdbId = series.Id,
                    imageType = "Logo",
                    filePath = logoImage.Logos[0].FilePath
                };
                if (!await _seriesDetailService.ExistsAsync(item.MediaId))
                {
                    await _imagesService.AddAsync(poster);
                    await _imagesService.AddAsync(backdrop);
                    await _imagesService.AddAsync(logo);
                    await _iCacheService.CacheImageAsync(series.PosterPath);
                    await _iCacheService.CacheImageAsync(series.BackdropPath);
                    await _iCacheService.CacheImageAsync(logoImage.Logos[0].FilePath);
                }
            }
            
            // 获取 season
            var season = await _tmdbClient.GetTvSeasonAsync(TvId, seasonNumber, language: "zh-CN");
            if (season == null)
            {
                Console.WriteLine($"未能获取季信息：{seriesName} S{seasonNumber}");
                continue;
            }
            var se = await _itemService.CreateOrGetMediaItemAsync(item);
            bool seasonExists = await _seasonService.ExistsAsync(item.MediaId, seasonNumber);
            if (!seasonExists)
            {
                var seasonItem = new Season
                {
                    MediaId = se.MediaId,
                    SeasonNumber = seasonNumber,
                    SeasonName = season.Name,
                    overview = season.Overview ?? string.Empty,
                    AirDate = season.AirDate ?? DateTime.MinValue,
                    posterPath = season.PosterPath ?? string.Empty,
                    rating = (float)season.VoteAverage,
                    episodeCount = season.Episodes.Count
                };

                await _seasonService.AddSeasonAsync(seasonItem);
            }
            bool existsSeasonPoster = await _imagesService.ExistsAsync(
                tmdbId: season.Id ?? 0,
                imageType: "SeasonPoster"
            );

            var seasonPoster = new Data.media.Models.Images
            {
                tmdbId = item.TMDbId,
                imageType = "SeasonPoster",
                filePath = season.PosterPath,
            };
            //if (!existsSeasonPoster)
            await _imagesService.AddAsync(seasonPoster);

            // 获取该剧集指定季集的元数据
            var epData = await _tmdbClient.GetTvEpisodeAsync(TvId, seasonNumber, episodeNumber);
            if (epData == null)
            {
                Console.WriteLine($"未能获取剧集信息：{seriesName} S{seasonNumber} E{episodeNumber}");
                continue;
            }
            // 取得或创建 MediaItem
            var mediaItem = await _itemService.CreateOrGetMediaItemAsync(item);
            if (mediaItem.MediaId <= 0)
                throw new InvalidOperationException("MediaItem 无效");

            int mediaId = mediaItem.MediaId;
            int sNumber = epData.SeasonNumber;

            var seasonEntity = await _seasonService.CreateOrGetSeasonAsync(mediaId, sNumber, item.ReleaseDate);
            if (seasonEntity.SeasonId <= 0)
                throw new InvalidOperationException("Season 创建失败");
            bool b = await _episodeService.ExistsAsync(item.MediaId, seasonNumber, episodeNumber);
            if (!b)
            {
                var episodeItem = new Episodes
                {
                    mediaId = mediaId,
                    SeasonId = seasonEntity.SeasonId,
                    tmdbId = epData.Id ?? 0,
                    airDate = epData.AirDate ?? DateTime.MinValue,
                    seasonNumber = epData.SeasonNumber,
                    episodeNumber = epData.EpisodeNumber,
                    episodeName = epData.Name ?? string.Empty,
                    duration = epData.Runtime ?? 0,
                    overview = epData.Overview ?? string.Empty,
                    stillPath = epData.StillPath ?? string.Empty,
                    rating = (float)epData.VoteAverage,
                };
                await _episodeService.CreateAsync(episodeItem);
                var episodeCrew = epData.Credits.Cast
                    .Select(c => new MediaCast
                    {
                        tmdbId     = epData.Id ?? 0,
                        Name       = c.Name,
                        Department = c.Character,
                        PersonUrl  = c.ProfilePath
                    })
                    .ToList();
                episodeCrew.AddRange(
                    epData.Credits.Crew
                        .Select(c => new MediaCast
                        {
                            tmdbId     = epData.Id ?? 0,
                            Name       = c.Name,
                            Department = c.Job,
                            PersonUrl  = c.ProfilePath
                        })
                );
                await _castService.CreateAsync(episodeCrew);
            
                var episodeFile = new Files
                {
                    tmdbId = epData.Id ?? 0,
                    //playhistory = 0,
                    filePath = file
                };
                await _context.Files.AddAsync(episodeFile);
                await _iCacheService.CacheImageAsync(epData.StillPath);
            }
            
        }
        
    }*/
}