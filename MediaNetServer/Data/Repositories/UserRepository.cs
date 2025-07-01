using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(string username, string hashedPassword, string email);
        Task<bool> UpdatePasswordAsync(string userId, string newHashedPassword);
        Task<bool> ValidateUserCredentialsAsync(string username, string hashedPassword);
        Task<Token?> GetTokenAsync(string refreshToken);
        Task<Token> CreateTokenAsync(string userId, string accessToken, string refreshToken, DateTime expiresAt);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<bool> CleanupExpiredTokensAsync();
    }

    public class UserRepository : IUserRepository
    {
        private readonly MediaContext _context;

        public UserRepository(MediaContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateUserAsync(string username, string hashedPassword, string email)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                PasswordHash = hashedPassword,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdatePasswordAsync(string userId, string newHashedPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = newHashedPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string hashedPassword)
        {
            var user = await GetUserByUsernameAsync(username);
            return user != null && user.PasswordHash == hashedPassword;
        }

        public async Task<Token?> GetTokenAsync(string refreshToken)
        {
            return await _context.Tokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<Token> CreateTokenAsync(string userId, string accessToken, string refreshToken, DateTime expiresAt)
        {
            // 先清理用户的旧token
            var oldTokens = await _context.Tokens
                .Where(t => t.UserId == userId)
                .ToListAsync();
            _context.Tokens.RemoveRange(oldTokens);

            var token = new Token
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.Tokens
                .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
            
            if (token == null) return false;

            _context.Tokens.Remove(token);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.Tokens
                .Where(t => t.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.Tokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
