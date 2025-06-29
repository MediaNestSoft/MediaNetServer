using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;  // 引入 User 类所在的命名空间
using System;
using System.Linq;

namespace MediaNetServer.Data.media.Services
{
    public class UserService
    {
        private readonly MediaContext _context;

        public UserService(MediaContext context)
        {
            _context = context;
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
    }
}
