using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaNetServer.Data.media.Models
{
    [Table("PlaylistItems")]
    public class PlaylistItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int playlistItemId { get; set; } // 主键，自增

        [ForeignKey("Playlist")]
        public int playlistId { get; set; }     // 外键 → Playlists

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; }        // 外键 → MediaItems

        public DateTime addedAt { get; set; }

        public DateTime releaseDate { get; set; }

        // 导航属性
        public Playlist Playlist { get; set; }

        public MediaItem MediaItem { get; set; }
    }
}
