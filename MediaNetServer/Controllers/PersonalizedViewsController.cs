using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using Org.OpenAPITools.Client;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("personal")]
public class PersonalizedViewsController : ControllerBase
{
    private readonly WatchProgressService _watchProgressService;
    private readonly MediaItemService _mediaItemService;
    private readonly ILogger<PersonalizedViewsController> _logger;
    private readonly PlaylistService _playlistService;
    private readonly EpisodesService _episodesSvc;

    public PersonalizedViewsController(WatchProgressService watchProgressService, 
        MediaItemService mediaItemService, ILogger<PersonalizedViewsController> logger,
        PlaylistService playlistService, EpisodesService episodesSvc)
    {
        _watchProgressService = watchProgressService;
        _mediaItemService = mediaItemService;
        _logger = logger;
        _playlistService = playlistService;
        _episodesSvc = episodesSvc;
    }

    [HttpGet("continue-watching")]
    public async Task<IActionResult> GetContinueWatching([FromQuery]string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var watchProgresses = await _watchProgressService.GetContinueWatchingAsync(userId, limit, offset);

            var response = new ContinueWatchResponse
            {
                Items = watchProgresses,
                TotalCount = watchProgresses.Count
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
    public async Task<IActionResult> GetRecentAdds([FromQuery]string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var recentMedia = await _mediaItemService.GetRecentlyAddedAsync(limit, offset);

            var mediaItems = new List<MediaItem>();
            foreach (var m in recentMedia)
            {
                // 判断是不是电影
                bool isMovie = string.Equals(m.Type, "movie", StringComparison.OrdinalIgnoreCase);

                // 直接用枚举成员，不要写数字
                var typeEnum = isMovie
                    ? MediaItem.TypeEnum.Movie
                    : MediaItem.TypeEnum.Series;

                // 根据类型取不同的 additional
                string? additionalInfo;
                if (isMovie)
                {
                    additionalInfo = m.ReleaseDate.Year.ToString();
                }
                else
                {
                    var episode = await _episodesSvc.GetByEpisodeImdbIdAsync(m.TMDbId);
                    additionalInfo = episode?.seasonNumber.ToString();
                }

                mediaItems.Add(new MediaItem(
                    mediaId:    new Option<int?>(m.TMDbId),
                    title:      new Option<string?>(m.Title),
                    type:       new Option<MediaItem.TypeEnum?>(typeEnum),
                    posterPath: new Option<string?>(m.PosterPath),
                    additional: !string.IsNullOrEmpty(additionalInfo)
                        ? new Option<string?>(additionalInfo)
                        : default
                ));
            }

            var totalCount = mediaItems.Count;
            var response = new MediaListResponse
            {
                Items      = mediaItems,
                TotalCount = totalCount
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
