using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using Org.OpenAPITools.Client;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;
using MediaNetServer.Models;

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

    [HttpGet("recent-add")]
    public async Task<IActionResult> GetRecentAdds([FromQuery]string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var allMedia = await _mediaItemService.GetRecentlyAddedAsync();

            int totalCount = allMedia.Count;
            int skip       = offset * limit;
            if (skip < 0) skip = 0;

            var pageMedia = allMedia
                .Skip(skip)
                .Take(limit)
                .ToList();

            var mediaItems = new List<LocalMediaItem>();
            foreach (var m in pageMedia)
            {
                bool isMovie = string.Equals(m.Type, "movie", StringComparison.OrdinalIgnoreCase);
                var typeEnum = isMovie
                    ? MediaType.Movie
                    : MediaType.Series;

                string? additional = isMovie
                    ? m.ReleaseDate.Year.ToString()
                    : (await _episodesSvc.GetByEpisodeImdbIdAsync(m.TMDbId))?.seasonNumber.ToString();

                mediaItems.Add(new LocalMediaItem(
                    tmDbId:    m.TMDbId,
                    title:      m.Title,
                    type:        typeEnum,
                    posterPath: m.PosterPath,
                    additional: additional
                ));
            }

            var response = new LocalMediaListResponse(
                mediaItems,
                totalCount
                );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent adds for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
    
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("pong");
    
    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommended([FromQuery]string userId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var allMedia = await _playlistService.GetRecommendedAsync();

            int totalCount = allMedia.Count;
            int skip       = offset * limit;
            if (skip < 0) skip = 0;

            var pageMedia = allMedia
                .Skip(skip)
                .Take(limit)
                .ToList();

            var mediaItems = new List<LocalMediaItem>();
            foreach (var m in pageMedia)
            {
                bool isMovie = string.Equals(m.Type, "movie", StringComparison.OrdinalIgnoreCase);
                var typeEnum = isMovie
                    ? MediaType.Movie
                    : MediaType.Series;

                string? additional = isMovie
                    ? m.ReleaseDate.Year.ToString()
                    : (await _episodesSvc.GetByEpisodeImdbIdAsync(m.TMDbId))?.seasonNumber.ToString();

                mediaItems.Add(new LocalMediaItem(
                    tmDbId:    m.TMDbId,
                    title:      m.Title,
                    type:       typeEnum,
                    posterPath: m.PosterPath,
                    additional: additional
                ));
            }

            var response = new LocalMediaListResponse(
                mediaItems,
                totalCount
                );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
}
