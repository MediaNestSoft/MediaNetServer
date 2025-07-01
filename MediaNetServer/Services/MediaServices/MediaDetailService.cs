using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Services.Folder;
using Microsoft.EntityFrameworkCore;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Services.MediaServices;
public class MediaDetailService
{
    private readonly FolderScraperService _scraper;
    private readonly MediaContext         _context;

    public MediaDetailService(FolderScraperService scraper, MediaContext context)
    {
        _scraper = scraper;
        _context = context;
    }
    

    /// <summary>
    /// 刮削并持久化媒体详情
    /// </summary>
    public async Task ProcessMediaAsync(string rootPath)
    {
        var (movies, episodes) = await _scraper.ScrapeFolderAsync(rootPath);

        // 1. 处理电影
        foreach (var movie in movies)
        {
            
            // 插入 MediaItem
            var item = new MediaItem
            {
                TMDbId       = movie.Id,
                Title        = movie.Title,
                Type         = "Movie",
                PosterPath   = movie.PosterPath ?? string.Empty,
                BackdropPath = movie.BackdropPath ?? string.Empty,
                LocalPath    = Path.Combine(rootPath, "Movies"),
                Rating       = movie.VoteAverage,
                ReleaseDate  = movie.ReleaseDate ?? DateTime.MinValue,
                Country      = movie.ProductionCountries.FirstOrDefault()?.Iso_3166_1 ?? string.Empty
            };
            _context.MediaItems.Add(item);
            await _context.SaveChangesAsync();

            // 插入 MovieDetail
            var detail = new MovieDetail
            {
                MediaId  = item.MediaId,
                Overview = movie.Overview ?? string.Empty,
                Duration = movie.Runtime ?? 0
            };
            _context.MovieDetails.Add(detail);
        }
        await _context.SaveChangesAsync();

        // 2. 处理电视剧集
        foreach (var ep in episodes)
        {
            // 获取或创建系列 MediaItem
            var mediaItem = await _context.MediaItems
                .FirstOrDefaultAsync(mi => mi.TMDbId == ep.ShowId);

            if (mediaItem == null)
            {
                mediaItem = new MediaItem
                {
                    TMDbId       = ep.ShowId,
                    Title        = ep.Name,
                    Type         = "Series",
                    PosterPath   = ep.,
                    BackdropPath = string.Empty,
                    LocalPath    = Path.Combine(rootPath, "Series"),
                    Rating       = 0,
                    ReleaseDate  = ep.AirDate ?? DateTime.MinValue,
                    Country      = string.Empty
                };
                _context.MediaItems.Add(mediaItem);
                await _context.SaveChangesAsync();

                // 插入 SeriesDetail
                var seriesDetail = new SeriesDetail
                {
                    mediaId           = mediaItem.MediaId,
                    firstAirDate      = ep.AirDate ?? DateTime.MinValue,
                    numberOfSeasons   = 0,
                    numberOfEpisodes  = 0
                };
                //_context.SeriesDetails.Add(seriesDetail);
                //await _context.SaveChangesAsync();
            }

            // 获取或创建 Season
            var season = await _context.Seasons
                .FirstOrDefaultAsync(s => s.MediaId == mediaItem.MediaId && s.SeasonNumber == ep.SeasonNumber);
            if (season == null)
            {
                season = new Season
                {
                    MediaId      = mediaItem.MediaId,
                    SeasonNumber = ep.SeasonNumber,
                    SeasonName   = $"Season {ep.SeasonNumber}"
                };
                _context.Seasons.Add(season);
                await _context.SaveChangesAsync();
            }

            // 插入一集
            var epEntity = new Episodes
            {
                mediaId       = mediaItem.MediaId,
                SeasonId      = season.SeasonId,
                episodeNumber = ep.EpisodeNumber,
                episodeName   = ep.Name ?? string.Empty,
                duration      = ep.Runtime ?? 0,
                overview      = ep.Overview ?? string.Empty,
                stillPath     = ep.StillPath ?? string.Empty
            };
            _context.Episodes.Add(epEntity);
        }
        await _context.SaveChangesAsync();
    }
}