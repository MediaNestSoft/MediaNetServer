using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Media.Models
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


        public int? episodeNumber { get; set; }

        // 导航属性
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem MediaItem { get; set; }

        
    }
}
