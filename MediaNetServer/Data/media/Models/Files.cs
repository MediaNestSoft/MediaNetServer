using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediaNetServer.Data.media.Models;

namespace MediaNetServer.Data.media.Models
{
    [Table("Files")]
    public class Files
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fid { get; set; } // 自增主键

        public string fileId { get; set; } // 文件 ID

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 外键：关联 MediaItems

        public int playhistory { get; set; } // 播放进度（秒）

        public string filePath { get; set; } // 文件路径

        // 导航属性
        public MediaItem MediaItem { get; set; }
    }
}
