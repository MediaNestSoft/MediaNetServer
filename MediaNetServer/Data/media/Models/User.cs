using System;
using System.ComponentModel.DataAnnotations;

namespace Media.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }  // 主键（GUID）

       
        public string Username { get; set; }  // 用户名

        
        public string PasswordHash { get; set; }  // 用户密码哈希

        
        public bool IsAdmin { get; set; }  // 是否为管理员（0 否，1 是）
    }
}
