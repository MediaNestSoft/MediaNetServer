using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Media.Models;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly FilesService _service;

    public FilesController(FilesService service)
    {
        _service = service;
    }

    // GET: api/files
    [HttpGet]
    public async Task<ActionResult<List<Files>>> GetAll()
    {
        var files = await _service.GetAllAsync();
        return Ok(files);
    }

    // GET: api/files/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Files>> GetById(int id)
    {
        var file = await _service.GetByIdAsync(id);
        if (file == null) return NotFound();
        return Ok(file);
    }

    // POST: api/files
    [HttpPost]
    public async Task<ActionResult<Files>> Create(Files file)
    {
        var created = await _service.CreateAsync(file);
        return CreatedAtAction(nameof(GetById), new { id = created.fid }, created);
    }

    // PUT: api/files/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Files file)
    {
        if (id != file.fid) return BadRequest();

        var updated = await _service.UpdateAsync(id, file);
        if (!updated) return NotFound();

        return NoContent();
    }

    // DELETE: api/files/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
