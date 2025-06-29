using Microsoft.AspNetCore.Mvc;
using Media.Models;
using Media.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly GenreService _service;

        public GenreController(GenreService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{gid}")]
        public async Task<ActionResult<Genre>> Get(int gid)
        {
            var genre = await _service.GetByIdAsync(gid);
            if (genre == null) return NotFound();
            return Ok(genre);
        }

        [HttpPost]
        public async Task<ActionResult<Genre>> Create([FromBody] Genre genre)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(genre);
            return CreatedAtAction(nameof(Get), new { gid = created.Gid }, created);
        }

        [HttpPut("{gid}")]
        public async Task<IActionResult> Update(int gid, [FromBody] Genre genre)
        {
            if (gid != genre.Gid)
                return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(gid, genre);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{gid}")]
        public async Task<IActionResult> Delete(int gid)
        {
            var deleted = await _service.DeleteAsync(gid);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
