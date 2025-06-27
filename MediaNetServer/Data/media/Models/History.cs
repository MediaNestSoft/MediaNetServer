using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [Table("History")]
    public class History
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int historyId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; }

        public DateTime watchedAt { get; set; }

        public int position { get; set; }

        public int duration { get; set; }

        public int seasonNumber { get; set; }

        public int episodeNumber { get; set; }

        public bool isFinished { get; set; }

        // 导航属性
        public User User { get; set; }
        public MediaItem MediaItem { get; set; }
    }
}
