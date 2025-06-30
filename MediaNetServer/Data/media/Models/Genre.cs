using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaNetServer.Data.media.Models
{
    public class Genre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Gid { get; set; }  // 主键，自增 int

        public int genreId { get; set; } // 外部 TMDb 的 genre id

        public string Name { get; set; } // 流派名称
    }
}
