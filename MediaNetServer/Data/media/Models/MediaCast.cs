using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class MediaCast
    {
        [Key]
        public int PersonId { get; set; } // 主键

        [ForeignKey("MediaItem")]
        public int MediaId { get; set; } // 外键关联 MediaItems

        public string Name { get; set; } // 演员姓名

        public string Department { get; set; } // 角色名称或职位

        public string PersonUrl { get; set; } // 演员图像

        // 导航属性
        public MediaItem MediaItem { get; set; }
    }
}
