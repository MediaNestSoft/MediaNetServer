using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MediaNetServer.Data.Media.Models;

namespace Media.Models
{
    [Table("Files")]
    public class Files
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fid { get; set; } // 自增主键

        public string fileId { get; set; } // 文件 ID

        [ForeignKey("MediaItem")]
        public int mediaId { get; set; } // 外键：关联 MediaItems

        [ForeignKey("Folder")]
        public Guid? FolderId { get; set; } // 新增：外键，关联 Folders，可为空

        public int playhistory { get; set; } // 播放进度（秒）

        public string filePath { get; set; } // 文件路径

        // 导航属性：MediaItem
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public MediaItem MediaItem { get; set; }

        // 新增：导航属性 Folders
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public Folders? Folder { get; set; }
    }
}
