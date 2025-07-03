using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;
using Org.OpenAPITools.Client;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly MediaItemService _mediaItemService;
    private readonly ILogger<SearchController> _logger;
    private readonly SeasonService _seasonService;

    public SearchController(MediaItemService mediaItemService, ILogger<SearchController> logger,
        SeasonService seasonService)
    {
        _mediaItemService = mediaItemService;
        _logger = logger;
        _seasonService = seasonService;
    }

    [HttpGet("title")]
    public async Task<IActionResult> SearchByTitle([FromQuery] string q, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var allMatches = await _mediaItemService.SearchByTitleAsync(q);
            var matched = new List<MediaItem>();

            var totalCount = allMatches.Count;

            var paged = allMatches
                .OrderBy(m => m.Title)
                .Skip(offset * limit)
                .Take(limit)
                .ToList();
            
            var mediaItems = new List<MediaItem>();
            foreach (var m in paged)
            {
                bool isMovie = string.Equals(m.Type, "movie", StringComparison.OrdinalIgnoreCase);
                var typeEnum = isMovie
                    ? MediaItem.TypeEnum.Movie
                    : MediaItem.TypeEnum.Series;
                var seasonNum = await _seasonService.GetSeasonsByMediaIdAsync(m.TMDbId);

                string? additional = isMovie
                    ? m.ReleaseDate.Year.ToString()
                    : seasonNum[0].SeasonNumber.ToString();

                mediaItems.Add(new MediaItem(
                    mediaId:    new Option<int?>(m.TMDbId),
                    title:      new Option<string?>(m.Title),
                    type:       new Option<MediaItem.TypeEnum?>(typeEnum),
                    posterPath: new Option<string?>(m.PosterPath),
                    additional: new Option<string?>(additional)
                ));
            }

            var response = new SearchResponse
            {
                Items    = mediaItems,
                TotalCount = totalCount
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for {Query}", q);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> GetAutocomplete([FromQuery] string q, [FromQuery] int limit = 20)
    {
        try
        {
            var mediaItems = await _mediaItemService.SearchByTitleAsync(q);
            var suggestions = mediaItems.Select(m => m.Title).Distinct().Take(limit).ToList();

            var response = new AutocompleteResponse
            {
                Suggestions = suggestions
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting autocomplete for {Query}", q);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
}
