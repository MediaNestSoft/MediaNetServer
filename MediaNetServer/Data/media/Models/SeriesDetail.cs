using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    [Table("SeriesDetail")]
    public class SeriesDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // 主键，自增
        
        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 外键

        public DateTime firstAirDate { get; set; } // 首播日期
        
        public DateTime lastAirDate { get; set; } // 最后播出日期

        public int numberOfSeasons { get; set; } // 季数

        public int numberOfEpisodes { get; set; } // 集数
        
        public string overview { get; set; } // 简介

        // 导航属性
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem MediaItem { get; set; }
    }
}
