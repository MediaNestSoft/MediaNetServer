using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Media.Models
{
    public class MovieDetail
    {
        [Key, ForeignKey("MediaItem")]
        public int MediaId { get; set; } // 主键 & 外键

        public string Overview { get; set; } // 概述

        public int Duration { get; set; } // 时长（秒）

        // 导航属性
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem MediaItem { get; set; }
    }
}
