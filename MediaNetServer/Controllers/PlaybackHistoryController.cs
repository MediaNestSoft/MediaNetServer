using MediaNetServer.Data.media.Models;
using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using MediaNetServer.Models;
using Microsoft.Extensions.Logging;
using Org.OpenAPITools.Client;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("playback")]
public class PlaybackHistoryController : ControllerBase
{
    private readonly HistoryService _historyService;
    private readonly WatchProgressService _watchProgressService;
    private readonly MediaItemService _mediaItemService;
    private readonly ILogger<PlaybackHistoryController> _logger;
    private readonly MovieDetailService _movieDetailService;
    private readonly EpisodesService _episodesService;
    private readonly SeasonService _seasonService;

    public PlaybackHistoryController(HistoryService historyService, WatchProgressService watchProgressService,
        MediaItemService mediaItemService, ILogger<PlaybackHistoryController> logger, MovieDetailService movieDetailService,
        EpisodesService episodesService, SeasonService seasonService)
    {
        _historyService = historyService;
        _watchProgressService = watchProgressService;
        _mediaItemService = mediaItemService;
        _logger = logger;
        _movieDetailService = movieDetailService;
        _episodesService = episodesService;
        _seasonService = seasonService;
    }

    [HttpGet("history/all")]
    public async Task<IActionResult> GetPlaybackHistory([FromQuery]string userId, [FromQuery]int limit, [FromQuery]int offset)
    {
        try
        {
            var histories = await _historyService.GetAllHistoryByUserIdAsync(userId);
            var historyItems = new List<LocalPlaybackHistoryItem>();

            foreach (var history in histories)
            {
                // MediaItem
                var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(history.tmdbId);
                if (mediaItem == null) 
                    continue;

                // 判断类型
                if (mediaItem.Type.Equals("movie", StringComparison.OrdinalIgnoreCase))
                {
                    // Movie
                    var movieDetail = await _movieDetailService.GetByMediaIdAsync(mediaItem.TMDbId);
                    historyItems.Add(new LocalPlaybackHistoryItem
                    {
                        MediaId    = mediaItem.TMDbId,
                        Title      = mediaItem.Title,
                        Type       = MediaType.Movie,
                        PosterPath = mediaItem.PosterPath,
                        Additional = mediaItem.ReleaseDate.Year.ToString(),
                        SeasonNumber = -1,
                        EpisodeNumber = -1,
                        Position   = history.position ?? 0,
                        Runtime    = movieDetail.Duration * 60
                    });
                }
                else if (mediaItem.Type.Equals("series", StringComparison.OrdinalIgnoreCase))
                {
                    // Series
                    var seasonNum  = history.seasonNumber ?? -1;
                    var episodeNum = history.episodeNumber ?? -1;

                    var episode = await _episodesService
                        .GetEpisode(
                            tmdbId:    mediaItem.TMDbId,
                            seasonNumber: seasonNum,
                            episodeNumber: episodeNum
                        );

                    historyItems.Add(new LocalPlaybackHistoryItem
                    {
                        MediaId    = mediaItem.TMDbId,
                        Title      = mediaItem.Title,
                        Type       = MediaType.Series,
                        PosterPath = mediaItem.PosterPath,
                        Additional = seasonNum.ToString(),
                        SeasonNumber = seasonNum,
                        EpisodeNumber = episodeNum,
                        Position   = history.position ?? 0,
                        Runtime    = episode.duration * 60
                    });
                }
            }
            var totalCount = historyItems.Count;

            var skip = offset * limit;
            if (skip < 0) skip = 0;

            // 分页：跳过 skip 条，取 limit 条
            var pageItems = historyItems
                .Skip(skip)
                .Take(limit)
                .ToList();

            var response = new LocalPlaybackHistoryResponse
            {
                Items = pageItems,
                TotalCount = totalCount,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting playback history for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("history/movie")]
    public async Task<IActionResult> GetMoviePlaybackHistory([FromQuery]string userId, [FromQuery]int mediaId)
    {
        try
        {
            var histories = await _historyService.GetHistoryByUserIdAsync(userId, mediaId);
            var movieDetail = await _movieDetailService.GetByMediaIdAsync(mediaId);
            
            var response = new LocalPlaybackHistoryItem
            {
                MediaId = mediaId,
                Position = histories.FirstOrDefault()?.position ?? 0,
                Runtime = histories.FirstOrDefault().duration
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movie playback history for user {UserId}, media {SeasonId}", userId, mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("history/series")]
    public async Task<IActionResult> GetSeriesPlaybackHistory(string userId, int mediaId)
    {
        try
        {
            var histories = await _historyService.GetHistoryByUserIdAsync(userId, mediaId);

            var response = new SeriesPlaybackHistory
            {
                MediaId = mediaId,
                SeasonNumber = histories[0].seasonNumber ?? 0,
                EpisodeNumber = histories[0].episodeNumber ?? 0,
                Position = histories[0].position ?? 0,
                Runtime = histories.FirstOrDefault().duration,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting series playback history for user {UserId}, media {SeasonId}", userId, mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost("progress")]
    public async Task<IActionResult> ReportPlaybackProgress([FromQuery]string userId, [FromBody]LocalReportPlaybackRequest reportPlaybackRequest)
    {
        try
        {
            var userGuid = Guid.Parse(userId);

            var progress = new WatchProgress
            {
                UserId        = userGuid,
                tmdbId        = reportPlaybackRequest.MediaId,
                position      = reportPlaybackRequest.Position,
                lastWatched   = DateTime.UtcNow,
                seasonNumber  = reportPlaybackRequest.SeasonNumber ?? -1,
                episodeNumber = reportPlaybackRequest.EpisodeNumber ?? -1
            };

            await _watchProgressService
                .UpdateOrCreateAsync(progress)
                .ConfigureAwait(false);

            
            var history = new History
            {
                UserId     = userGuid,
                tmdbId     = reportPlaybackRequest.MediaId,
                position   = reportPlaybackRequest.Position,
                watchedAt  = DateTime.UtcNow,
                seasonNumber  = reportPlaybackRequest.SeasonNumber,
                episodeNumber = reportPlaybackRequest.EpisodeNumber,
                isFinished    = false
            };

            await _historyService
                .UpdateOrCreateHistoryAsync(history)
                .ConfigureAwait(false);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting playback progress for user {UserId}", userId);
            return StatusCode(204, new Error { Message = "Internal server error" });
        }
    }
}
