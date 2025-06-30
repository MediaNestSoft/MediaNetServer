using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly HistoryService _historyService;

        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<History>>> GetAll()
        {
            return await _historyService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<History>> GetById(int id)
        {
            var history = await _historyService.GetByIdAsync(id);
            if (history == null) return NotFound();
            return history;
        }

        [HttpPost]
        public async Task<ActionResult<History>> Create(History history)
        {
            var created = await _historyService.CreateAsync(history);
            return CreatedAtAction(nameof(GetById), new { id = created.historyId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<History>> Update(int id, History updated)
        {
            var result = await _historyService.UpdateAsync(id, updated);
            if (result == null) return NotFound();
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _historyService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
