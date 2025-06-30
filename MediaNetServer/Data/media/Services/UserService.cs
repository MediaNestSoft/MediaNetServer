using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
// 引入 User 类所在的命名空间
// 引入集合命名空间

namespace MediaNetServer.Data.media.Services
{
    public class UserService
    {
        private readonly MediaContext _context;

        public UserService(MediaContext context)
        {
            _context = context;
        }

        // 查询所有用户
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        // 查询用户（通过ID）
        public User GetUserById(Guid userId)
        {
            return _context.Users.FirstOrDefault(user => user.UserId == userId);
        }

        // 查询用户（通过用户名）
        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(user => user.Username == username);
        }

        // 添加用户
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // 更新用户
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        // 删除用户
        public void DeleteUser(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        
        /// <summary>
        /// 验证用户名和密码是否匹配。
        /// </summary>
        /// <returns>如果用户名不存在，返回 null；存在但密码错误，返回 null；都正确返回对应的 User。</returns>
        public User? ValidateCredentials(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null)
                return null;

            bool ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return ok ? user : null;
        }
    }
}
