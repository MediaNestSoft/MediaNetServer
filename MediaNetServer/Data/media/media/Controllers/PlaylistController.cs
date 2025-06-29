using Media.Models;
using Media.Services;
using Microsoft.AspNetCore.Mvc;

namespace Media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistService _service;

        public PlaylistController(PlaylistService service)
        {
            _service = service;
        }

        // GET: api/Playlist
        [HttpGet]
        public async Task<ActionResult<List<Playlist>>> GetAll()
        {
            var playlists = await _service.GetAllAsync();
            return Ok(playlists);
        }

        // GET: api/Playlist/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetById(int id)
        {
            var playlist = await _service.GetByIdAsync(id);
            if (playlist == null) return NotFound();
            return Ok(playlist);
        }

        // POST: api/Playlist
        [HttpPost]
        public async Task<ActionResult<Playlist>> Create(Playlist playlist)
        {
            var created = await _service.AddAsync(playlist);
            return CreatedAtAction(nameof(GetById), new { id = created.playlistId }, created);
        }

        // PUT: api/Playlist/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Playlist playlist)
        {
            if (id != playlist.playlistId) return BadRequest("ID 不匹配");

            var updated = await _service.UpdateAsync(playlist);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/Playlist/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
