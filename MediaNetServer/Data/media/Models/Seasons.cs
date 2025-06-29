using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaNetServer.Data.media.Models
{
    [Table("Seasons")]
    public class Seasons
    {
        [Key]
        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 主键 + 外键

        public int seasonNumber { get; set; } // 季数

        public string seasonName { get; set; } // 季名称

        // 导航属性
        public MediaItem MediaItem { get; set; }
    }
}
