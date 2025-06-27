using Microsoft.EntityFrameworkCore;
using Media.Models;

namespace Media.Data
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
        public DbSet<Seasons> Seasons { get; set; }
        public DbSet<Episodes> Episodes { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistItems> PlaylistItems { get; set; }
        public DbSet<WatchProgress> WatchProgress { get; set; }
        public DbSet<History> History { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Episodes>()
                .HasOne(e => e.MediaItem)
                .WithMany()
                .HasForeignKey(e => e.mediaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Episodes>()
                .HasOne(e => e.Season)
                .WithMany()
                .HasForeignKey(e => e.seasonNumber)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
