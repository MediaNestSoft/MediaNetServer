using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediaNetServer.Data.media.Models
{
    public class Folders
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Path { get; set; } = null!;

        // 与 Files 的一对多关系
        [BindNever]
        [JsonIgnore]
        [ValidateNever]
        public ICollection<Files> Items { get; set; } = new List<Files>();
    }
}
