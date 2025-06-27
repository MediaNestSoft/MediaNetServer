using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    [Table("MediaGenres")]
    public class MediaGenres
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int mediaGenreId { get; set; } // 主键，自增

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 外键，关联 MediaItems.mediaId

        [ForeignKey("Genre")]
        public int genreId { get; set; } // 外键，关联 Genres.gid（你要求的变量名）

        // 导航属性
        public MediaItem MediaItem { get; set; }

        public Genre Genre { get; set; }
    }
}
