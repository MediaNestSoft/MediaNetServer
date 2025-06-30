using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistItemsController : ControllerBase
    {
        private readonly PlaylistItemsService _service;

        public PlaylistItemsController(PlaylistItemsService service)
        {
            _service = service;
        }

        // GET: api/PlaylistItems/playlist/5
        [HttpGet("playlist/{playlistId}")]
        public async Task<ActionResult<List<PlaylistItems>>> GetByPlaylistId(int playlistId)
        {
            var items = await _service.GetByPlaylistIdAsync(playlistId);
            return Ok(items);
        }

        // GET: api/PlaylistItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaylistItems>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/PlaylistItems
        [HttpPost]
        public async Task<ActionResult<PlaylistItems>> Create(PlaylistItems item)
        {
            var created = await _service.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = created.playlistItemId }, created);
        }

        // PUT: api/PlaylistItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PlaylistItems item)
        {
            if (id != item.playlistItemId) return BadRequest("ID 不匹配");

            var updated = await _service.UpdateAsync(item);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/PlaylistItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
