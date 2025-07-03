using System.Collections.Generic;  // 新增
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    [Table("Seasons")]
    public class Season
    {
        [Key]
        public int SeasonId { get; set; } // 主键 tmdb

        [ForeignKey("MediaItem")]
        public int MediaId { get; set; }  // 外键，关联 MediaItems

        public int SeasonNumber { get; set; } // 季数

        public string? SeasonName { get; set; } // 季名称
        
        public string overview { get; set; } // 季概述
        
        public DateTime AirDate { get; set; }
        
        public int? episodeCount { get; set; } // 集数
        
        public string? posterPath { get; set; } // 海报路径
        
        public float rating { get; set; } // 评分

        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem? MediaItem { get; set; } // 导航属性（可选）

        // 新增：导航属性，表示这个季包含多个剧集
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<Episodes> Episodes { get; set; } = new HashSet<Episodes>();

    }
}
