using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    public class MediaItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaId { get; set; } // 主键

        public int TMDbId { get; set; } // TMDb的媒体ID

        public string Title { get; set; } // 媒体标题

        public string Type { get; set; } // 媒体类型（电影、电视剧）

        public string? PosterPath { get; set; } // 媒体海报路径

        public string? BackdropPath { get; set; } // 背景图路径
        
        public string? LogoPath { get; set; } // Logo路径

        public string? LocalPath { get; set; } // 本地存储路径

        public double Rating { get; set; } // 媒体评分

        public DateTime ReleaseDate { get; set; } // 发行日期

        public string? Language { get; set; } // 语言
        
        public List<string>? Genre { get; set; } // 流派列表
        
        public string? Country { get; set; } // 国家
        
        public DateTime AddTime { get; set; } //媒体的添加时间

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
