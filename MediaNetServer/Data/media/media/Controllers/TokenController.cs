using Media.Models;  // 引入 User 模型
using Media.Services;  // 引入 Token 服务
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserService _userService;

        public TokenController(TokenService tokenService, UserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        // 登录并获取 Token
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _userService.GetUserByUsername(loginRequest.Username);

            if (user == null || user.PasswordHash != loginRequest.Password)
            {
                return Unauthorized("Invalid credentials");  // 用户名或密码错误
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });  // 返回生成的 Token
        }

        // 刷新 Token
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var principal = _tokenService.ValidateToken(refreshTokenRequest.Token);
            if (principal == null)
            {
                return Unauthorized("Invalid token");
            }

            var userId = principal.FindFirstValue("sub");  // 从 Token 中获取用户 ID
            var user = _userService.GetUserById(Guid.Parse(userId));

            if (user == null)
            {
                return Unauthorized("Invalid token");
            }

            var newToken = _tokenService.GenerateToken(user);  // 生成新的 Token
            return Ok(new { token = newToken });
        }
    }

    // 请求体：用于登录的请求
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // 请求体：用于刷新 Token 的请求
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
    }
}
