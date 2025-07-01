using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly MediaItemService _mediaItemService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(MediaItemService mediaItemService, ILogger<SearchController> logger)
    {
        _mediaItemService = mediaItemService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> SearchByTitle([FromQuery] string q, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var mediaItems = await _mediaItemService.SearchByTitleAsync(q, limit, offset);
            var searchResults = mediaItems.Select(m => new MediaOverview
            {
                Id = m.Id,
                Title = m.Title,
                Type = m.Type,
                PosterPath = m.PosterPath,
                Rating = m.Rating,
                ReleaseDate = m.ReleaseDate
            }).ToList();

            var response = new SearchResponse
            {
                Results = searchResults,
                TotalCount = searchResults.Count, // 实际应该获取总数
                Query = q
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
            var mediaItems = await _mediaItemService.SearchByTitleAsync(q, limit, 0);
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
