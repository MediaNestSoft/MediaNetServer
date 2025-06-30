using System;
using System.Linq;
using System.Text.RegularExpressions;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaNetServer.Services.AuthorizationServices;

public class UserAuthService
{
    private readonly UserService _userService;

    public UserAuthService(UserService userService)
    {
        _userService = userService;
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
            UserId = Guid.NewGuid(),
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsAdmin = isAdmin
        };

        _userService.AddUser(user);
        Console.WriteLine(isAdmin ? "管理员用户创建成功" : "普通用户创建成功");
    }

    public void PromoteUserInteractive()
    {
        Console.WriteLine("请输入要升级为管理员的用户名:");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;
        var user = _userService.GetUserByUsername(username);
        if (user == null)
        {
            Console.WriteLine("用户不存在");
            return;
        }
        if (user.IsAdmin)
        {
            Console.WriteLine("该用户已是管理员");
            return;
        }
        user.IsAdmin = true;
        _userService.UpdateUser(user);
        Console.WriteLine("用户已升级为管理员");
    }
}