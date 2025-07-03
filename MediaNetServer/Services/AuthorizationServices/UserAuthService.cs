using System;
using System.Linq;
using System.Text.RegularExpressions;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Services.AuthorizationServices
{
    public class UserAuthService
    {
        private readonly UserService _userService;

        public UserAuthService(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 启动一次性交互式认证流程
        /// </summary>
        public void StartInteractive()
        {
            Console.WriteLine("请输入操作指令:[R] 注册， [L] 已注册，在客户端登录登录");
            var key = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (key == "R")
            {
                CreateUserInteractive();
            }
            else if (key == "L")
            {
                Console.WriteLine("请前往客户端进行登录。");
            }
            else
            {
                Console.WriteLine("输入无效，未执行任何操作。");
            }
        }

        public void CreateUserInteractive()
        {
            Console.WriteLine("请创建用户名 (英文字母/数字/下划线, 最多6位):");
            string username;
            while (true)
            {
                username = Console.ReadLine()?.Trim() ?? string.Empty;
                if (Regex.IsMatch(username, "^[A-Za-z0-9_]{1,6}$"))
                {
                    if (_userService.GetUserByUsername(username) == null)
                        break;
                    Console.WriteLine("用户名已存在，请重新输入:");
                }
                else
                {
                    Console.WriteLine("用户名非法，请重新输入:");
                }
            }

            Console.WriteLine("请创建密码 (6-15位, 包含大写、小写、数字, 不含其他符号):");
            string password;
            while (true)
            {
                password = Console.ReadLine()?.Trim() ?? string.Empty;
                if (Regex.IsMatch(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{6,15}$"))
                    break;
                Console.WriteLine("密码不符合要求，请重新输入:");
            }

            bool isAdmin = !_userService.GetAllUsers().Any();

            var user = new User
            {
                UserId       = Guid.NewGuid(),
                Username     = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsAdmin      = isAdmin
            };

            _userService.AddUser(user);
            Console.WriteLine("用户创建成功");
        }
    }
}