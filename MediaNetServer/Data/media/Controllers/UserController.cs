using Microsoft.AspNetCore.Mvc;
using MediaNetServer.Data.media.Services;  // 引入服务
using MediaNetServer.Data.media.Models;    // 引入模型层

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

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(Guid id)
        {
            var user = _userService.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public ActionResult CreateUser([FromBody] User user)
        {
            _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
    }
}
