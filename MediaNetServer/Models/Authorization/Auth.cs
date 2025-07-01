using Org.OpenAPITools.Api;

namespace MediaNetServer.Models.Authorization;
public class LoginRequestDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required string UserId { get; set; }
}

public class RefreshRequestDto
{
    public required string RefreshToken { get; set; }
}

public class LogoutRequestDto
{
    public required string RefreshToken { get; set; }
}

public class ChangePasswordRequestDto
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}