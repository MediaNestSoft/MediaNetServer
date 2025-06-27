using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [Table("SeriesDetail")]
    public class SeriesDetail
    {
        [Key]
        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 主键 & 外键

        public DateTime firstAirDate { get; set; } // 首播日期

        public int numberOfSeasons { get; set; } // 季数

        public int numberOfEpisodes { get; set; } // 集数

        // 导航属性
        public MediaItem MediaItem { get; set; }
    }
}
