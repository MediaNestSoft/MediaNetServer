using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    [Table("WatchProgress")]
    public class WatchProgress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int watchProgressId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; }

        public DateTime lastWatched { get; set; }

        public int position { get; set; } // 播放秒数

        public int seasonNumber { get; set; } // 剧集：季数，电影为 0

        public int episodeNumber { get; set; } // 剧集：集数，电影为 0

        // 导航属性
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public User User { get; set; }

        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem MediaItem { get; set; }
    }
}
