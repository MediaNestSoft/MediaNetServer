using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Data.media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaItemController : ControllerBase
    {
        private readonly MediaItemService _service;

        public MediaItemController(MediaItemService service)
        {
            _service = service;
        }

        // GET 所有媒体项
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MediaItem>>> GetAllMediaItems()
        {
            return await _service.GetAllMediaItemsAsync();
        }

        // GET 单个媒体项
        [HttpGet("{id}")]
        public async Task<ActionResult<MediaItem>> GetMediaItemById(int id)
        {
            var item = await _service.GetMediaItemByIdAsync(id);
            if (item == null)
                return NotFound();
            return item;
        }

        // POST 创建媒体项
        [HttpPost]
        public async Task<ActionResult<MediaItem>> CreateMediaItem([FromBody] MediaItem mediaItem)
        {
            if (mediaItem == null)
                return BadRequest("无效的数据");

            var created = await _service.CreateMediaItemAsync(mediaItem);
            return CreatedAtAction(nameof(GetMediaItemById), new { id = created.MediaId }, created);
        }

        // PUT 更新媒体项
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMediaItem(int id, [FromBody] MediaItem mediaItem)
        {
            if (id != mediaItem.MediaId)
                return BadRequest("ID 不匹配");

            var updated = await _service.UpdateMediaItemAsync(id, mediaItem);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE 删除媒体项
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMediaItem(int id)
        {
            var deleted = await _service.DeleteMediaItemAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
