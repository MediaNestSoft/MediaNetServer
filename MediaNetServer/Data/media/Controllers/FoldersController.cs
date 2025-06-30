using MediaNetServer.Data.media.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoldersController : ControllerBase
    {
        private readonly FoldersService _service;

        public FoldersController(FoldersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Folders folder)
        {
            var result = await _service.CreateAsync(folder);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Folders folder)
        {
            if (id != folder.Id) return BadRequest();

            var success = await _service.UpdateAsync(folder);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
