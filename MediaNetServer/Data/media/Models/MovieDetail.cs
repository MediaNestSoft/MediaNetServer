using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class MovieDetail
    {
        [Key, ForeignKey("MediaItem")]
        public int MediaId { get; set; } // 主键 & 外键

        public string Overview { get; set; } // 概述

        public int Duration { get; set; } // 时长（秒）

        // 导航属性
        public MediaItem MediaItem { get; set; }
    }
}
