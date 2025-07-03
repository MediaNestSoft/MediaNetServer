using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        public int tmdbId { get; set; }        // 外键 → MediaItems

        public DateTime addedAt { get; set; }

        public DateTime releaseDate { get; set; }

        // 导航属性
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public Playlist Playlist { get; set; }

        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem MediaItem { get; set; }
    }
}
