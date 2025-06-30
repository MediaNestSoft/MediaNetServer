using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 根据 ID 获取用户
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(Guid id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
                return NotFound(); // 如果找不到用户，返回 404
            return Ok(user); // 返回用户数据
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAllUsers(); // 获取所有用户
            return Ok(users); // 返回用户列表
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        [HttpPost]
        public ActionResult CreateUser([FromBody] User user)
        {
            _userService.AddUser(user); // 调用服务层添加用户
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user); // 返回 201 创建成功响应
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        [HttpPut("{id}")]
        public ActionResult UpdateUser(Guid id, [FromBody] User user)
        {
            var existingUser = _userService.GetUserById(id);
            if (existingUser == null)
                return NotFound(); // 如果用户不存在，返回 404

            // 这里可以选择更新字段，比如用户名和密码哈希等
            existingUser.Username = user.Username;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.IsAdmin = user.IsAdmin;

            _userService.UpdateUser(existingUser); // 更新用户信息
            return NoContent(); // 返回 204 无内容，表示更新成功
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(Guid id)
        {
            var existingUser = _userService.GetUserById(id);
            if (existingUser == null)
                return NotFound(); // 如果用户不存在，返回 404

            _userService.DeleteUser(id); // 调用服务层删除用户
            return NoContent(); // 返回 204 无内容，表示删除成功
        }
    }
}
