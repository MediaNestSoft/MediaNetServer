using System;
using System.ComponentModel.DataAnnotations;

namespace Media.Models
{
    public class MediaItem
    {
        [Key]
        public int MediaId { get; set; } // 主键

        public int TMDbId { get; set; } // TMDb的媒体ID

        public string Title { get; set; } // 媒体标题

        public string Type { get; set; } // 媒体类型（电影、电视剧）

        public string PosterPath { get; set; } // 媒体海报路径

        public string BackdropPath { get; set; } // 背景图路径

        public string LocalPath { get; set; } // 本地存储路径

        public double Rating { get; set; } // 媒体评分

        public DateTime ReleaseDate { get; set; } // 发行日期

        public string Country { get; set; } // 发行国家
    }
}
