using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;  // 必须加
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
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

        // 下面是新增的导航属性，告诉EF Core这个媒体项有多个剧集
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<Episodes> Episodes { get; set; } = new HashSet<Episodes>();
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<Season> Seasons { get; set; } = new HashSet<Season>();

    }
}
