using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data
{
    public class MediaNetDbContext : DbContext
    {
        public MediaNetDbContext(DbContextOptions<MediaNetDbContext> options) : base(options)
        {
        }

        // 这里添加你需要的 DbSet 属性
        // public DbSet<User> Users { get; set; }
        // public DbSet<Media> Media { get; set; }
        // public DbSet<Playlist> Playlists { get; set; }
        // 等等...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 在这里配置实体关系
        }
    }
}
