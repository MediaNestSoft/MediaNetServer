using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    [Table("History")]
    public class History
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int historyId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public int tmdbId { get; set; }

        public DateTime? watchedAt { get; set; }

        public int? position { get; set; }

        public int duration { get; set; }

        public int? seasonNumber { get; set; }

        public int? episodeNumber { get; set; }

        public bool isFinished { get; set; }

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
