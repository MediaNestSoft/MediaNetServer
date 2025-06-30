using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieDetailController : ControllerBase
    {
        private readonly MovieDetailService _service;

        public MovieDetailController(MovieDetailService service)
        {
            _service = service;
        }

        // GET: api/MovieDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDetail>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // GET: api/MovieDetail/5
        [HttpGet("{mediaId:int}")]
        public async Task<ActionResult<MovieDetail>> Get(int mediaId)
        {
            var detail = await _service.GetByMediaIdAsync(mediaId);
            if (detail == null) return NotFound();
            return Ok(detail);
        }

        // POST: api/MovieDetail
        [HttpPost]
        public async Task<ActionResult<MovieDetail>> Create([FromBody] MovieDetail detail)
        {
            // ✅ 跳过对导航属性 MediaItem 的验证（关键行）
            ModelState.Remove("MediaItem");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(detail);
            if (created == null)
                return BadRequest($"MediaItem with ID={detail.MediaId} does not exist.");

            return CreatedAtAction(nameof(Get), new { mediaId = created.MediaId }, created);
        }

        // PUT: api/MovieDetail/5
        [HttpPut("{mediaId:int}")]
        public async Task<IActionResult> Update(int mediaId, [FromBody] MovieDetail detail)
        {
            if (mediaId != detail.MediaId)
                return BadRequest("URL mediaId and body MediaId must match.");

            var updated = await _service.UpdateAsync(mediaId, detail);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE: api/MovieDetail/5
        [HttpDelete("{mediaId:int}")]
        public async Task<IActionResult> Delete(int mediaId)
        {
            var deleted = await _service.DeleteAsync(mediaId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
