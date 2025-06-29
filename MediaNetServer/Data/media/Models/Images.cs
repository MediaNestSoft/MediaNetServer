using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaNetServer.Data.media.Models
{
    [Table("Images")]
    public class Images
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int imageId { get; set; }

        public string imageType { get; set; }

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; }

        public string filePath { get; set; }

        public string size { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        [ForeignKey("Episode")]
        public int? episodeId { get; set; }  // 可空，只有剧集图才用

        // 导航属性
        public MediaItem MediaItem { get; set; }

        public Episodes Episode { get; set; }
    }
}
