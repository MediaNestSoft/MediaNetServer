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
        /// 启动命令行：登录或注册
        /// </summary>
        public void StartInteractive()
        {
            while (true)
            {
                Console.WriteLine("请选择操作: [L] 登录，[R] 注册，[Q] 退出");
                var key = Console.ReadLine()?.Trim().ToUpperInvariant();
                if (key == "L")
                {
                    if (LoginInteractive()) break;
                }
                else if (key == "R")
                {
                    CreateUserInteractive();
                }
                else if (key == "Q")
                {
                    Environment.Exit(0);
                }
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

            // 第一个用户为管理员
            bool isAdmin = !_userService.GetAllUsers().Any();

            var user = new User
            {
                UserId       = Guid.NewGuid(),
                Username     = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsAdmin      = isAdmin
            };

            _userService.AddUser(user);
            Console.WriteLine(isAdmin ? "管理员用户创建成功" : "普通用户创建成功");
        }

        /// <summary>
        /// 登录，成功后返回 true 并退出循环，密码错误超过3次返回 false
        /// </summary>
        private bool LoginInteractive()
        {
            Console.WriteLine("请输入用户名:");
            var username = Console.ReadLine()?.Trim() ?? string.Empty;
            var user = _userService.GetUserByUsername(username);
            if (user == null)
            {
                Console.WriteLine("用户名不存在，请先注册。");
                return false;
            }

            for (int tries = 0; tries < 3; tries++)
            {
                Console.WriteLine("请输入密码:");
                var pwd = Console.ReadLine()?.Trim() ?? string.Empty;
                var valid = _userService.ValidateCredentials(username, pwd);
                if (valid != null)
                {
                    Console.WriteLine($"登录成功，欢迎{valid.Username}!  {(valid.IsAdmin? "[管理员]":"")}");
                    return true;
                }

                Console.WriteLine("密码错误，请重试。");
            }

            Console.WriteLine("尝试次数过多，返回主菜单。");
            return false;
        }
    }
}