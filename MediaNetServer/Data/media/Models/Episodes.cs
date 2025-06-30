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
        public int mediaId { get; set; } // 外键，关联 MediaItems

        [ForeignKey("Season")]
        public int SeasonId { get; set; } // 外键，关联 Seasons

        public int episodeNumber { get; set; } // 集数
        public string episodeName { get; set; } // 集名称
        public int duration { get; set; } // 时长（秒）
        public string overview { get; set; } // 集概述
        public string stillPath { get; set; } // 海报路径

        // 导航属性（可选）
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual MediaItem MediaItem { get; set; }
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual Season Season { get; set; }
    }
}
