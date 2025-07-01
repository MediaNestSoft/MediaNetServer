using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("users/{userId}/views")]
public class PersonalizedViewsController : ControllerBase
{
    private readonly WatchProgressService _watchProgressService;
    private readonly MediaItemService _mediaItemService;
    private readonly ILogger<PersonalizedViewsController> _logger;

    public PersonalizedViewsController(WatchProgressService watchProgressService, 
        MediaItemService mediaItemService, ILogger<PersonalizedViewsController> logger)
    {
        _watchProgressService = watchProgressService;
        _mediaItemService = mediaItemService;
        _logger = logger;
    }

    [HttpGet("continue-watching")]
    public async Task<IActionResult> GetContinueWatching(string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var watchProgresses = await _watchProgressService.GetContinueWatchingAsync(userId, limit, offset);
            var continueWatchItems = new List<ContinueWatchItem>();

            foreach (var wp in watchProgresses)
            {
                var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(wp.MediaId);
                if (mediaItem != null)
                {
                    continueWatchItems.Add(new ContinueWatchItem
                    {
                        MediaId = mediaItem.Id,
                        Title = mediaItem.Title,
                        Type = mediaItem.Type,
                        PosterPath = mediaItem.PosterPath,
                        Progress = wp.Position,
                        Duration = wp.Duration,
                        LastWatched = wp.UpdatedAt
                    });
                }
            }

            var response = new ContinueWatchResponse
            {
                Items = continueWatchItems,
                TotalCount = continueWatchItems.Count
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting continue watching for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("recent-adds")]
    public async Task<IActionResult> GetRecentAdds(string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var recentMedia = await _mediaItemService.GetRecentlyAddedAsync(limit, offset);
            var mediaItems = recentMedia.Select(m => new MediaOverview
            {
                Id = m.Id,
                Title = m.Title,
                Type = m.Type,
                PosterPath = m.PosterPath,
                Rating = m.Rating,
                ReleaseDate = m.ReleaseDate
            }).ToList();

            var response = new MediaListResponse
            {
                Media = mediaItems,
                TotalCount = mediaItems.Count,
                Page = offset / limit + 1,
                TotalPages = (int)Math.Ceiling((double)mediaItems.Count / limit)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent adds for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
}
