using MediaNetServer.Data.media.Models;
using MediaNetServer.Services.MediaServices;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Data
{
    public class MediaContext : DbContext
    {
        public MediaContext(DbContextOptions<MediaContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<MovieDetail> MovieDetails { get; set; }
        public DbSet<MediaCast> MediaCasts { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MediaGenres> MediaGenres { get; set; }
        public DbSet<SeriesDetail> SeriesDetail { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistItems> PlaylistItems { get; set; }
        public DbSet<WatchProgress> WatchProgress { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<Episodes> Episodes { get; set; }
        public DbSet<Folders> Folders { get; set; }
        public DbSet<WatchProgress> WatchProgresses { get; set; }
        
        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            // 对所有新增或修改的 WatchProgress 实体，更新 UpdatedAt
            var entries = ChangeTracker
                .Entries<WatchProgress>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.lastWatched = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<MediaCast>()
                .HasOne(mc => mc.MediaItem)
                .WithMany()
                .HasPrincipalKey(mi => mi.TMDbId)
                .HasForeignKey(mc => mc.tmdbId)
                .OnDelete(DeleteBehavior.Cascade);

            // Episodes -> MediaItem
            modelBuilder.Entity<Episodes>()
                .HasOne(e => e.MediaItem)
                .WithMany(m => m.Episodes)
                .HasForeignKey(e => e.mediaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Episodes -> Season
            modelBuilder.Entity<Episodes>()
                .HasOne(e => e.Season)
                .WithMany(s => s.Episodes)
                .HasForeignKey(e => e.SeasonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Season -> MediaItem
            modelBuilder.Entity<Season>()
                .HasOne(s => s.MediaItem)
                .WithMany(m => m.Seasons)
                .HasForeignKey(s => s.MediaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Files -> MediaItem
            modelBuilder.Entity<Files>()
                .HasOne(f => f.MediaItem)
                .WithMany()
                .HasPrincipalKey(m => m.TMDbId)
                .HasForeignKey(f => f.tmdbId)
                .OnDelete(DeleteBehavior.Restrict);

            // Files -> Folders
            modelBuilder.Entity<Files>()
                .HasOne(f => f.Folder)
                .WithMany(folder => folder.Items)
                .HasForeignKey(f => f.FolderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Images -> MediaItem
            modelBuilder.Entity<Images>()
                .HasOne(i => i.MediaItem)
                .WithMany()
                .HasPrincipalKey(m => m.TMDbId)       // 指定主表的外键字段是 TMDbId，而不是默认的主键
                .HasForeignKey(i => i.tmdbId)         // 指定 Images 这边用 tmdbId 作为外键
                .OnDelete(DeleteBehavior.Cascade);

            // PlaylistItems -> Playlist
            modelBuilder.Entity<PlaylistItems>()
                .HasOne(pi => pi.Playlist)
                .WithMany()
                .HasForeignKey(pi => pi.playlistId)
                .OnDelete(DeleteBehavior.Restrict);

            // PlaylistItems -> MediaItem
            modelBuilder.Entity<PlaylistItems>()
                .HasOne(pi => pi.MediaItem)
                .WithMany()
                .HasPrincipalKey(m => m.TMDbId)
                .HasForeignKey(pi => pi.tmdbId)
                .OnDelete(DeleteBehavior.Restrict);

            // Playlists -> User
            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // WatchProgress -> MediaItem
            modelBuilder.Entity<WatchProgress>()
                .HasOne(wp => wp.MediaItem)
                .WithMany()
                .HasPrincipalKey(m  => m.TMDbId)
                .HasForeignKey(wp => wp.tmdbId)
                .OnDelete(DeleteBehavior.Restrict);

            // WatchProgress -> User
            modelBuilder.Entity<WatchProgress>()
                .HasOne(wp => wp.User)
                .WithMany()
                .HasForeignKey(wp => wp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // History -> MediaItem
            modelBuilder.Entity<History>()
                .HasOne(h => h.MediaItem)
                .WithMany()
                .HasPrincipalKey(h => h.TMDbId)
                .HasForeignKey(h => h.tmdbId)
                .OnDelete(DeleteBehavior.Restrict);

            // History -> User
            modelBuilder.Entity<History>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
