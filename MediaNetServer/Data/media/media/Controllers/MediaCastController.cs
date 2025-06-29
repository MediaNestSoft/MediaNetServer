using Media.Models;
using Media.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Controllers
{
    // API 路由：用于处理与 MediaCast 相关的请求
    [Route("api/[controller]")]
    [ApiController]
    public class MediaCastController : ControllerBase
    {
        private readonly MediaCastService _service;

        // 构造函数，注入 MediaCastService
        public MediaCastController(MediaCastService service)
        {
            _service = service;
        }

        // 获取所有的 MediaCast 数据（所有演员信息）
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MediaCast>>> GetAll()
        {
            // 调用 service 层获取所有 MediaCast 信息，并返回 200 OK
            return Ok(await _service.GetAllAsync());
        }

        // 根据 personId 获取单个 MediaCast 数据
        [HttpGet("{personId:int}")]
        public async Task<ActionResult<MediaCast>> Get(int personId)
        {
            // 调用 service 层通过 personId 获取 MediaCast
            var cast = await _service.GetByPersonIdAsync(personId);
            if (cast == null) return NotFound(); // 如果没有找到，返回 404

            return Ok(cast); // 返回找到的 MediaCast 对象
        }

        // 创建新的 MediaCast 数据
        [HttpPost]
        public async Task<ActionResult<MediaCast>> Create([FromBody] MediaCast cast)
        {
            // 移除任何可能的模型验证问题（例如 MediaItem）
            ModelState.Remove("MediaItem");

            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 如果模型无效，返回 400 错误

            // 调用 service 层创建新的 MediaCast
            var created = await _service.CreateAsync(cast);
            if (created == null)
                return BadRequest($"MediaItem with ID={cast.MediaId} does not exist."); // 如果 MediaItem 不存在，返回错误

            // 返回创建成功的 MediaCast 对象，同时返回 201 状态码
            return CreatedAtAction(nameof(Get), new { personId = created.PersonId }, created);
        }

        // 更新 MediaCast 信息
        [HttpPut("{personId:int}")]
        public async Task<IActionResult> Update(int personId, [FromBody] MediaCast cast)
        {
            // 确保 URL 中的 personId 与请求体中的一致
            if (personId != cast.PersonId)
                return BadRequest("URL id and body id do not match.");

            // 调用 service 层更新 MediaCast
            var updated = await _service.UpdateAsync(personId, cast);
            if (!updated) return NotFound(); // 如果没有找到要更新的 MediaCast，返回 404

            return NoContent(); // 返回 204（无内容，表示成功更新）
        }

        // 删除指定 personId 的 MediaCast 数据
        [HttpDelete("{personId:int}")]
        public async Task<IActionResult> Delete(int personId)
        {
            // 调用 service 层删除 MediaCast
            var deleted = await _service.DeleteAsync(personId);
            if (!deleted) return NotFound(); // 如果删除失败，返回 404

            return NoContent(); // 返回 204（无内容，表示成功删除）
        }
    }
}
