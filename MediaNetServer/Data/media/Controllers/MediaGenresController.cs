using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaGenresController : ControllerBase
    {
        private readonly MediaGenresService _service;

        public MediaGenresController(MediaGenresService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MediaGenres>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MediaGenres>> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MediaGenres>> Create(MediaGenres mg)
        {
            var created = await _service.CreateAsync(mg);
            return CreatedAtAction(nameof(Get), new { id = created.mediaGenreId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MediaGenres mg)
        {
            var success = await _service.UpdateAsync(id, mg);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
