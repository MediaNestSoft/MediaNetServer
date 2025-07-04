﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    [Table("Playlists")]
    public class Playlist  
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pId { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }  
        
        public int playlistId { get; set; } // 播放列表标识，可重复

        [MaxLength(255)]
        public string name { get; set; }

        public bool isSystem { get; set; }

        // 导航属性
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public User User { get; set; }
    }
}
