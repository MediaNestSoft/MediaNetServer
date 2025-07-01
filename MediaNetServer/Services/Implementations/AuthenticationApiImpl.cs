using MediaNetServer.Data.Repositories;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;
using Org.OpenAPITools.Client;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace MediaNetServer.Services.Implementations
{
    public class AuthenticationApiImpl : IAuthenticationApi
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationApiImpl> _logger;

        public AuthenticationApiImpl(IUserRepository userRepository, ILogger<AuthenticationApiImpl> logger, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _logger = logger;
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }
        
        public AuthenticationApiEvents Events { get; } = new AuthenticationApiEvents();

        public async Task<IChangePasswordApiResponse> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                // 这里需要从请求上下文中获取当前用户ID
                // 假设从JWT token中获取
                var userId = "current-user-id"; // 实际应该从认证上下文获取
                
                var success = await _userRepository.UpdatePasswordAsync(userId, BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword));
                
                if (success)
                {
                    return new ChangePasswordApiResponse(System.Net.HttpStatusCode.OK, null, null);
                }
                else
                {
                    return new ChangePasswordApiResponse(System.Net.HttpStatusCode.BadRequest, null, 
                        new Error { Message = "Failed to change password" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return new ChangePasswordApiResponse(System.Net.HttpStatusCode.InternalServerError, null, 
                    new Error { Message = "Internal server error" });
            }
        }

        public async Task<IChangePasswordApiResponse?> ChangePasswordOrDefaultAsync(ChangePasswordRequest changePasswordRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await ChangePasswordAsync(changePasswordRequest, cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ILoginApiResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password);
                var isValid = await _userRepository.ValidateUserCredentialsAsync(loginRequest.Username, hashedPassword);
                
                if (isValid)
                {
                    var user = await _userRepository.GetUserByUsernameAsync(loginRequest.Username);
                    if (user != null)
                    {
                        // 生成JWT tokens
                        var accessToken = GenerateAccessToken(user.Id);
                        var refreshToken = GenerateRefreshToken();
                        var expiresAt = DateTime.UtcNow.AddHours(24);
                        
                        await _userRepository.CreateTokenAsync(user.Id, accessToken, refreshToken, expiresAt);
                        
                        var response = new LoginResponse
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            ExpiresIn = 86400, // 24 hours
                            TokenType = "Bearer"
                        };
                        
                        return new LoginApiResponse(System.Net.HttpStatusCode.OK, null, response);
                    }
                }
                
                return new LoginApiResponse(System.Net.HttpStatusCode.Unauthorized, null, 
                    new Error { Message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return new LoginApiResponse(System.Net.HttpStatusCode.InternalServerError, null, 
                    new Error { Message = "Internal server error" });
            }
        }

        public async Task<ILoginApiResponse?> LoginOrDefaultAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await LoginAsync(loginRequest, cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ILogoutApiResponse> LogoutAsync(LogoutRequest logoutRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var success = await _userRepository.RevokeTokenAsync(logoutRequest.RefreshToken);
                
                if (success)
                {
                    return new LogoutApiResponse(System.Net.HttpStatusCode.OK, null, null);
                }
                else
                {
                    return new LogoutApiResponse(System.Net.HttpStatusCode.BadRequest, null, 
                        new Error { Message = "Invalid refresh token" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return new LogoutApiResponse(System.Net.HttpStatusCode.InternalServerError, null, 
                    new Error { Message = "Internal server error" });
            }
        }

        public async Task<ILogoutApiResponse?> LogoutOrDefaultAsync(LogoutRequest logoutRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await LogoutAsync(logoutRequest, cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        public async Task<IRefreshTokenApiResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var token = await _userRepository.GetTokenAsync(refreshTokenRequest.RefreshToken);
                
                if (token != null)
                {
                    // 生成新的tokens
                    var newAccessToken = GenerateAccessToken(token.UserId);
                    var newRefreshToken = GenerateRefreshToken();
                    var expiresAt = DateTime.UtcNow.AddHours(24);
                    
                    await _userRepository.CreateTokenAsync(token.UserId, newAccessToken, newRefreshToken, expiresAt);
                    
                    var response = new LoginResponse
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        ExpiresIn = 86400,
                        TokenType = "Bearer"
                    };
                    
                    return new RefreshTokenApiResponse(System.Net.HttpStatusCode.OK, null, response);
                }
                
                return new RefreshTokenApiResponse(System.Net.HttpStatusCode.Unauthorized, null, 
                    new Error { Message = "Invalid refresh token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return new RefreshTokenApiResponse(System.Net.HttpStatusCode.InternalServerError, null, 
                    new Error { Message = "Internal server error" });
            }
        }

        public async Task<IRefreshTokenApiResponse?> RefreshTokenOrDefaultAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                return await RefreshTokenAsync(refreshTokenRequest, cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        private string GenerateAccessToken(string userId)
        {
            // 实际应该使用JWT库生成token
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{DateTime.UtcNow.Ticks}"));
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }

    // 响应类的实现
    public class ChangePasswordApiResponse : IChangePasswordApiResponse
    {
        public ChangePasswordApiResponse(System.Net.HttpStatusCode statusCode, string? reasonPhrase, Error? error)
        {
            StatusCode = (int)statusCode;
            ReasonPhrase = reasonPhrase;
            IsSuccessStatusCode = ((int)statusCode >= 200 && (int)statusCode < 300);
            
            if (statusCode == System.Net.HttpStatusCode.OK)
            {
                IsOk = true;
            }
            else if (statusCode == System.Net.HttpStatusCode.BadRequest)
            {
                IsBadRequest = true;
                _badRequestContent = error;
            }
        }

        public int StatusCode { get; }
        public string? ReasonPhrase { get; }
        public bool IsSuccessStatusCode { get; }
        public bool IsOk { get; }
        public bool IsBadRequest { get; }
        private readonly Error? _badRequestContent;
        
        public Error? BadRequest() => _badRequestContent;
    }

    // 其他响应类的实现会类似...
}
