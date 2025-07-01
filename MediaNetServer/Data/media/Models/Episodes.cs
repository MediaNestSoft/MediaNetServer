using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    [Table("Episodes")]
    public class Episodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int epId { get; set; } // 主键，自增
        
        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 关联的媒体项ID

        public int tmdbId { get; set; } // tmdbId
        
        public DateTime airDate { get; set; } // 首播日期
        
        [ForeignKey("Season")]
        public int SeasonId { get; set; } // 关联的季ID

        public int seasonNumber { get; set; } // 季数

        public int episodeNumber { get; set; } // 集数
        public string episodeName { get; set; } // 集名称
        public int duration { get; set; } // 时长（秒）
        public string overview { get; set; } // 集概述
        public string stillPath { get; set; } // 海报路径
        
        public double rating { get; set; } // 评分

        // 导航属性（可选）
        // 媒体外键，一对多
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual MediaItem MediaItem { get; set; } = null!;

        // 季度外键（可选，多级）
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual Season Season { get; set; } = null!;
    }
}
