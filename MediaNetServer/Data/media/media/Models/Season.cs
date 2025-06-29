using System.Collections.Generic;  // 新增
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Media.Models
{
    [Table("Seasons")]
    public class Season
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeasonId { get; set; } // 主键，自增

        [ForeignKey("MediaItem")]
        public int MediaId { get; set; }  // 外键，关联 MediaItems

        [Required]
        public int SeasonNumber { get; set; } // 季数

        [Required]
        public string SeasonName { get; set; } = string.Empty; // 季名称

        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem? MediaItem { get; set; } // 导航属性（可选）

        // 新增：导航属性，表示这个季包含多个剧集
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<Episodes> Episodes { get; set; } = new HashSet<Episodes>();

    }
}
