using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("users/{userId}/playlists")]
public class PlaylistController : ControllerBase
{
    private readonly PlaylistService _playlistService;
    private readonly PlaylistItemsService _playlistItemsService;
    private readonly ILogger<PlaylistController> _logger;

    public PlaylistController(PlaylistService playlistService, PlaylistItemsService playlistItemsService,
        ILogger<PlaylistController> logger)
    {
        _playlistService = playlistService;
        _playlistItemsService = playlistItemsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ListPlaylists(string userId)
    {
        try
        {
            var playlists = await _playlistService.GetByUserIdAsync(userId);
            var playlistList = playlists.Select(p => new Playlist
            {
                Id = p.playlistId,
                Name = p.name,
                Description = p.description ?? "",
                IsSystem = p.isSystem ?? false,
                CreatedAt = p.createdAt ?? DateTime.MinValue,
                UpdatedAt = p.updatedAt ?? DateTime.MinValue
            }).ToList();

            var response = new PlaylistsResponse
            {
                Playlists = playlistList
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting playlists for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlaylist(string userId, CreatePlaylistRequest request)
    {
        try
        {
            var playlist = new MediaNetServer.Data.media.Models.Playlist
            {
                UserId = Guid.Parse(userId),
                name = request.Name,
                description = request.Description,
                isSystem = false,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            };

            var createdPlaylist = await _playlistService.AddAsync(playlist);

            var response = new CreatePlaylistResponse
            {
                Id = createdPlaylist.pId,
                Name = createdPlaylist.name,
                Description = createdPlaylist.description
            };

            return Created($"/users/{userId}/playlists/{createdPlaylist.pId}", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating playlist for user {UserId}", userId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPut("{playlistId}/rename")]
    public async Task<IActionResult> RenamePlaylist(string userId, int playlistId, RenamePlaylistRequest request)
    {
        try
        {
            var playlist = await _playlistService.GetByIdAsync(playlistId);
            if (playlist == null || playlist.UserId.ToString() != userId)
            {
                return NotFound(new Error { Message = "Playlist not found" });
            }

            playlist.name = request.NewName;
            playlist.updatedAt = DateTime.UtcNow;

            var success = await _playlistService.UpdateAsync(playlist);
            if (success)
                return Ok();
            
            return BadRequest(new Error { Message = "Failed to rename playlist" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renaming playlist {PlaylistId}", playlistId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpDelete("{playlistId}")]
    public async Task<IActionResult> DeletePlaylist(string userId, int playlistId)
    {
        try
        {
            var playlist = await _playlistService.GetByIdAsync(playlistId);
            if (playlist == null || playlist.UserId.ToString() != userId)
            {
                return NotFound(new Error { Message = "Playlist not found" });
            }

            var success = await _playlistService.DeleteAsync(playlistId);
            if (success)
                return Ok();
            
            return NotFound(new Error { Message = "Failed to delete playlist" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting playlist {PlaylistId}", playlistId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost("{playlistId}/items")]
    public async Task<IActionResult> AddPlaylistItems(string userId, int playlistId, MediaIdsRequest request)
    {
        try
        {
            var playlist = await _playlistService.GetByIdAsync(playlistId);
            if (playlist == null || playlist.UserId.ToString() != userId)
            {
                return NotFound(new Error { Message = "Playlist not found" });
            }

            foreach (var mediaId in request.MediaIds)
            {
                var playlistItem = new MediaNetServer.Data.media.Models.PlaylistItems
                {
                    PlaylistId = playlistId,
                    MediaId = mediaId,
                    AddedAt = DateTime.UtcNow
                };
                await _playlistItemsService.AddAsync(playlistItem);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding items to playlist {PlaylistId}", playlistId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpDelete("{playlistId}/items")]
    public async Task<IActionResult> RemovePlaylistItems(string userId, int playlistId, MediaIdsRequest request)
    {
        try
        {
            var playlist = await _playlistService.GetByIdAsync(playlistId);
            if (playlist == null || playlist.UserId.ToString() != userId)
            {
                return NotFound(new Error { Message = "Playlist not found" });
            }

            foreach (var mediaId in request.MediaIds)
            {
                await _playlistItemsService.DeleteByPlaylistAndMediaAsync(playlistId, mediaId);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing items from playlist {PlaylistId}", playlistId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("{playlistId}/items")]
    public async Task<IActionResult> GetPlaylistItems(string userId, int playlistId, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        try
        {
            var playlist = await _playlistService.GetByIdAsync(playlistId);
            if (playlist == null || playlist.UserId.ToString() != userId)
            {
                return NotFound(new Error { Message = "Playlist not found" });
            }

            var playlistItems = await _playlistItemsService.GetByPlaylistIdAsync(playlistId);
            var mediaIds = playlistItems.Skip(offset).Take(limit).Select(pi => pi.MediaId).ToList();

            var response = new MediaIdsResponse
            {
                MediaIds = mediaIds,
                TotalCount = playlistItems.Count(),
                Page = offset / limit + 1,
                TotalPages = (int)Math.Ceiling((double)playlistItems.Count() / limit)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting playlist items for playlist {PlaylistId}", playlistId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpDelete("{playlistId}/clear")]
    public async Task<IActionResult> ClearPlaylist(string userId, int playlistId)
    {
        try
        {
            var playlist = await _playlistService.GetByIdAsync(playlistId);
            if (playlist == null || playlist.UserId.ToString() != userId)
            {
                return NotFound(new Error { Message = "Playlist not found" });
            }

            await _playlistItemsService.DeleteByPlaylistIdAsync(playlistId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing playlist {PlaylistId}", playlistId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
}
