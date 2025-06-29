using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaNetServer.Data.media.Models
{
    [Table("Playlists")]
    public class Playlist  
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int playlistId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }  

        [MaxLength(255)]
        public string name { get; set; }

        public bool isSystem { get; set; }

        // 导航属性
        public User User { get; set; }
    }
}
