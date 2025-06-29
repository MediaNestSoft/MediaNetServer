using Media.Models;
using Media.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeriesDetailController : ControllerBase
    {
        private readonly SeriesDetailService _service;

        public SeriesDetailController(SeriesDetailService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeriesDetail>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{mediaId}")]
        public async Task<ActionResult<SeriesDetail>> Get(int mediaId)
        {
            var result = await _service.GetByIdAsync(mediaId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SeriesDetail>> Create(SeriesDetail detail)
        {
            var created = await _service.CreateAsync(detail);
            return CreatedAtAction(nameof(Get), new { mediaId = created.mediaId }, created);
        }

        [HttpPut("{mediaId}")]
        public async Task<IActionResult> Update(int mediaId, SeriesDetail detail)
        {
            var success = await _service.UpdateAsync(mediaId, detail);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{mediaId}")]
        public async Task<IActionResult> Delete(int mediaId)
        {
            var success = await _service.DeleteAsync(mediaId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
