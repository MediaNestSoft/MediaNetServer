using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("users/{userId}/playback")]
public class PlaybackHistoryController : ControllerBase
{
    private readonly HistoryService _historyService;
    private readonly WatchProgressService _watchProgressService;
    private readonly MediaItemService _mediaItemService;
    private readonly ILogger<PlaybackHistoryController> _logger;

    public PlaybackHistoryController(HistoryService historyService, WatchProgressService watchProgressService,
        MediaItemService mediaItemService, ILogger<PlaybackHistoryController> logger)
    {
        _historyService = historyService;
        _watchProgressService = watchProgressService;
        _mediaItemService = mediaItemService;
        _logger = logger;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetPlaybackHistory(string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0, [FromQuery] string? mediaType = null)
    {
        try
        {
            var histories = await _historyService.GetHistoryByUserIdAsync(userId, limit, offset, mediaType);
            var historyItems = new List<PlaybackHistoryItem>();

            foreach (var history in histories)
            {
                var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(history.MediaId);
                if (mediaItem != null)
                {
                    historyItems.Add(new PlaybackHistoryItem
                    {
                        MediaId = mediaItem.Id,
                        Title = mediaItem.Title,
                        Type = mediaItem.Type,
                        PosterPath = mediaItem.PosterPath,
                        WatchedAt = history.WatchedAt,
                        Position = history.Position
                    });
                }
            }

            var response = new PlaybackHistoryResponse
            {
                History = historyItems,
                TotalCount = historyItems.Count,
                Page = offset / limit + 1,
                TotalPages = (int)Math.Ceiling((double)historyItems.Count / limit)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting playback history for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("movies/{tmdbId}/history")]
    public async Task<IActionResult> GetMoviePlaybackHistory(string userId, int mediaId)
    {
        try
        {
            var history = await _historyService.GetHistoryByUserAndMediaAsync(userId, mediaId);
            if (history == null)
            {
                return NotFound(new Error { Message = "Playback history not found" });
            }

            var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(mediaId);
            if (mediaItem?.Type != "movie")
            {
                return BadRequest(new Error { Message = "Media is not a movie" });
            }

            var response = new MoviePlaybackHistory
            {
                MediaId = mediaId,
                Title = mediaItem.Title,
                Position = history.Position,
                WatchedAt = history.WatchedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movie playback history for user {UserId}, media {MediaId}", userId, mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("series/{tmdbId}/history")]
    public async Task<IActionResult> GetSeriesPlaybackHistory(string userId, int mediaId)
    {
        try
        {
            var histories = await _historyService.GetSeriesHistoryByUserAndMediaAsync(userId, mediaId);
            var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(mediaId);
            
            if (mediaItem?.Type != "series")
            {
                return BadRequest(new Error { Message = "Media is not a series" });
            }

            var response = new SeriesPlaybackHistory
            {
                MediaId = mediaId,
                Title = mediaItem.Title,
                Episodes = histories.Select(h => new EpisodePlaybackHistory
                {
                    EpisodeId = h.EpisodeId ?? 0,
                    Position = h.Position,
                    WatchedAt = h.WatchedAt
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting series playback history for user {UserId}, media {MediaId}", userId, mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost("report")]
    public async Task<IActionResult> ReportPlaybackProgress(string userId, ReportPlaybackRequest request)
    {
        try
        {
            // 更新或创建观看进度
            var watchProgress = new MediaNetServer.Data.media.Models.WatchProgress
            {
                UserId = Guid.Parse(userId),
                MediaId = request.MediaId,
                Position = request.Position,
                Duration = request.Duration,
                UpdatedAt = DateTime.UtcNow
            };

            await _watchProgressService.UpdateOrCreateAsync(watchProgress);

            // 添加到历史记录
            var history = new MediaNetServer.Data.media.Models.History
            {
                UserId = Guid.Parse(userId),
                MediaId = request.MediaId,
                Position = request.Position,
                WatchedAt = DateTime.UtcNow
            };

            await _historyService.AddAsync(history);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting playback progress for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
}
