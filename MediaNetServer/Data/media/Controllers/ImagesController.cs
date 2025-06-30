using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ImagesService _service;

        public ImagesController(ImagesService service)
        {
            _service = service;
        }

        // GET: api/Images
        [HttpGet]
        public async Task<ActionResult<List<Images>>> GetAll()
        {
            var images = await _service.GetAllAsync();
            return Ok(images);
        }

        // GET: api/Images/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Images>> GetById(int id)
        {
            var image = await _service.GetByIdAsync(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        // POST: api/Images
        [HttpPost]
        public async Task<ActionResult<Images>> Create(Images image)
        {
            var created = await _service.AddAsync(image);
            return CreatedAtAction(nameof(GetById), new { id = created.imageId }, created);
        }

        // PUT: api/Images/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Images image)
        {
            if (id != image.imageId) return BadRequest("ID 不匹配");

            var result = await _service.UpdateAsync(image);
            if (!result) return NotFound();

            return NoContent();
        }

        // DELETE: api/Images/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
