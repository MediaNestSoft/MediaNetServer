using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.Repositories
{
    public interface IStreamingRepository
    {
        Task<Files?> GetFileByIdAsync(string fileId);
        Task<List<Files>> GetMovieFilesAsync(int mediaId);
        Task<List<Files>> GetEpisodeFilesAsync(int seriesId, int seasonNumber, int episodeNumber);
        Task<MediaItem?> GetMediaStatusAsync(string userId, int mediaId);
    }

    public class StreamingRepository : IStreamingRepository
    {
        private readonly MediaContext _context;

        public StreamingRepository(MediaContext context)
        {
            _context = context;
        }

        public async Task<Files?> GetFileByIdAsync(string fileId)
        {
            return await _context.Files
                .Include(f => f.MediaItem)
                .FirstOrDefaultAsync(f => f.Id == fileId);
        }

        public async Task<List<Files>> GetMovieFilesAsync(int mediaId)
        {
            return await _context.Files
                .Where(f => f.MediaId == mediaId)
                .OrderBy(f => f.Quality)
                .ToListAsync();
        }

        public async Task<List<Files>> GetEpisodeFilesAsync(int seriesId, int seasonNumber, int episodeNumber)
        {
            return await _context.Files
                .Include(f => f.Episode)
                    .ThenInclude(e => e.Season)
                .Where(f => f.Episode != null && 
                           f.Episode.Season.MediaId == seriesId && 
                           f.Episode.Season.SeasonNumber == seasonNumber && 
                           f.Episode.EpisodeNumber == episodeNumber)
                .OrderBy(f => f.Quality)
                .ToListAsync();
        }

        public async Task<MediaItem?> GetMediaStatusAsync(string userId, int mediaId)
        {
            return await _context.MediaItems
                .Include(m => m.WatchProgress.Where(wp => wp.UserId == userId))
                .FirstOrDefaultAsync(m => m.Id == mediaId);
        }
    }
}
