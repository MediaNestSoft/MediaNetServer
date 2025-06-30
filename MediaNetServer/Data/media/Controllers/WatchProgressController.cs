using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchProgressController : ControllerBase
    {
        private readonly WatchProgressService _service;

        public WatchProgressController(WatchProgressService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WatchProgress>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WatchProgress>> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<WatchProgress>> Create(WatchProgress watchProgress)
        {
            var created = await _service.CreateAsync(watchProgress);
            return CreatedAtAction(nameof(Get), new { id = created.watchProgressId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WatchProgress>> Update(int id, WatchProgress watchProgress)
        {
            var updated = await _service.UpdateAsync(id, watchProgress);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
