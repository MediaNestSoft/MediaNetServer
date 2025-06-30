using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeasonController : ControllerBase
    {
        private readonly SeasonService _seasonService;

        public SeasonController(SeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Season>>> GetAll()
        {
            return Ok(await _seasonService.GetAllSeasonsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Season>> GetById(int id)
        {
            var season = await _seasonService.GetSeasonByIdAsync(id);
            if (season == null) return NotFound();
            return Ok(season);
        }

        [HttpPost]
        public async Task<ActionResult<Season>> Create(Season season)
        {
            var created = await _seasonService.AddSeasonAsync(season);
            return CreatedAtAction(nameof(GetById), new { id = created.SeasonId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Season>> Update(int id, Season season)
        {
            var updated = await _seasonService.UpdateSeasonAsync(id, season);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _seasonService.DeleteSeasonAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
