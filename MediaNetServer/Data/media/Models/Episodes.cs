using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [Table("Episodes")]
    public class Episodes
    {
        [Key]
        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 主键 + 外键：MediaItems

        [ForeignKey("Season")]
        public int seasonNumber { get; set; } // 外键：Seasons

        public int episodeNumber { get; set; } // 集数

        public string episodeName { get; set; }

        public int duration { get; set; }

        public string overview { get; set; }

        public string stillPath { get; set; }

        // 导航属性
        public MediaItem MediaItem { get; set; }

        public Seasons Season { get; set; }
    }
}
