using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MediaNetServer.Data.media.Services
{
    public class TokenService
    {
        private readonly MediaContext _db;
        // 使用至少256位的密钥
        private readonly string _secretKey = "your_256_bit_secret_key_here_32_bytes_long_";

        // 生成 Token
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),  // 用户 ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // Token ID
                new Claim(ClaimTypes.Name, user.Username),  // 用户名
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")  // 角色
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));  // 确保密钥足够长
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourapp.com",
                audience: "yourapp.com",
                claims: claims,
                expires: DateTime.Now.AddHours(1),  // 设置 Token 过期时间
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);  // 返回 Token 字符串
        }

        // 验证 JWT Token
        public ClaimsPrincipal ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourapp.com",
                    ValidAudience = "yourapp.com",
                    IssuerSigningKey = key
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception)
            {
                return null;  // 如果验证失败，返回 null
            }
        }
        
        /// <summary>
        /// 添加用户令牌
        /// </summary>
        public async Task AddToken(Token token)
        {
            await _db.Tokens.AddAsync(token);
            await _db.SaveChangesAsync();
        }

        public async Task<Token?> GetTokenByRefreshToken(string refreshToken)
        {
            return await _db.Tokens
                .SingleOrDefaultAsync(t => t.RefreshToken == refreshToken);
        }

        public async Task DeleteToken(int tokenId)
        {
            var token = await _db.Tokens
                .SingleOrDefaultAsync(t => t.TokenId == tokenId);
            if (token == null)
            {
                throw new Exception("Token not found");
            }

            _db.Tokens.Remove(token);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateToken(Token token)
        {
            var existing = await _db.Tokens
                .FirstOrDefaultAsync(t => t.UserId == token.UserId);

            if (existing == null)
                throw new InvalidOperationException("查不到对应的 Token 记录");

            existing.AccessToken  = token.AccessToken;
            existing.RefreshToken = token.RefreshToken;
            existing.ExpiresAt    = token.ExpiresAt;

            await _db.SaveChangesAsync();
        }
    }
}
