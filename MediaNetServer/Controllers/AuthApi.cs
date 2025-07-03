using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserService userService, TokenService tokenService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var user = _userService.ValidateCredentials(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new Error { Message = "Invalid credentials" });
            }

            // 生成JWT tokens
            var accessToken = GenerateAccessToken(user.UserId.ToString());
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddHours(999);
            
            // 创建token记录
            var token = new Data.media.Models.Token
            {
                UserId = user.UserId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
            await _tokenService.AddToken(token);
            
            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                UserId = user.UserId.ToString()
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        try
        {
            var token = _tokenService.GetTokenByRefreshToken(request.RefreshToken);
            if (token != null)
            {
                await _tokenService.DeleteToken(token.Id);
                return Ok();
            }
            return BadRequest(new Error { Message = "Invalid refresh token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        try
        {
            var token = await _tokenService.GetTokenByRefreshToken(request.RefreshToken);
            if (token == null || token.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized(new Error { Message = "Invalid or expired refresh token" });
            }

            // 生成新的tokens
            var newAccessToken = GenerateAccessToken(token.UserId.ToString());
            var newRefreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddHours(24);
            
            // 更新token
            token.AccessToken = newAccessToken;
            token.RefreshToken = newRefreshToken;
            token.ExpiresAt = expiresAt;
            await _tokenService.UpdateToken(token);
            
            var response = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
                UserId = token.UserId.ToString()
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            // 这里需要从JWT token中获取当前用户ID
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var user = _userService.GetUserById(userId.Value);
            if (user == null)
                return NotFound(new Error { Message = "User not found" });

            // 验证旧密码
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                return BadRequest(new Error { Message = "Invalid old password" });

            // 更新密码
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _userService.UpdateUser(user);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    private string GenerateAccessToken(string userId)
    {
        // 简化的token生成，实际应该使用JWT库
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{DateTime.UtcNow.Ticks}"));
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    private Guid? GetCurrentUserId()
    {
        // 从JWT token中获取用户ID的实现
        // 这里需要实际的JWT解析逻辑
        return Guid.NewGuid(); // 临时实现
    }
}