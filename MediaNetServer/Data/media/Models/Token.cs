using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaNetServer.Data.media.Models
{
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; } 
        public Guid UserId { get; set; }  // 外键，关联到 Users 表
        public string AccessToken { get; set; }  // 访问令牌
        public string RefreshToken { get; set; }  // 刷新令牌
        public DateTime ExpiresAt { get; set; }  // 过期时间

        // 外键关系，可以在这里使用导航属性（如果需要）
        public User User { get; set; }  // 导航属性，表示此令牌属于哪个用户
    }
}
