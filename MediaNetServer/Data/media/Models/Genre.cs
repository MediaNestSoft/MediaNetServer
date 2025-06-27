using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Models
{
    public class Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Gid { get; set; }  // 主键，自增 int

        public string GenreId { get; set; } // 外部 TMDb 的 genre id

        public string Name { get; set; } // 流派名称
    }
}
