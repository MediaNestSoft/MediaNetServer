using Microsoft.AspNetCore.Mvc;
using Media.Models;
using Media.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EpisodesController : ControllerBase
    {
        private readonly EpisodesService _episodesService;

        public EpisodesController(EpisodesService episodesService)
        {
            _episodesService = episodesService;
        }

        // GET api/episodes
        [HttpGet]
        public async Task<ActionResult<List<Episodes>>> GetAll()
        {
            var episodes = await _episodesService.GetAllAsync();
            return Ok(episodes);
        }

        // GET api/episodes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Episodes>> GetById(int id)
        {
            var episode = await _episodesService.GetByIdAsync(id);
            if (episode == null) return NotFound();
            return Ok(episode);
        }

        // POST api/episodes
        [HttpPost]
        public async Task<ActionResult<Episodes>> Create(Episodes episode)
        {
            var created = await _episodesService.CreateAsync(episode);
            return CreatedAtAction(nameof(GetById), new { id = created.epId }, created);
        }

        // PUT api/episodes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Episodes episode)
        {
            if (id != episode.epId) return BadRequest();

            var result = await _episodesService.UpdateAsync(id, episode);
            if (!result) return NotFound();

            return NoContent();
        }

        // DELETE api/episodes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _episodesService.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
