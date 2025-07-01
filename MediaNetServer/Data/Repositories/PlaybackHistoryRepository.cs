using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.Repositories
{
    public interface IPlaybackHistoryRepository
    {
        Task<List<History>> GetPlaybackHistoryAsync(string userId, int limit, int offset, string? mediaType = null);
        Task<History?> GetMoviePlaybackHistoryAsync(string userId, int mediaId);
        Task<History?> GetSeriesPlaybackHistoryAsync(string userId, int mediaId);
        Task<bool> ReportPlaybackProgressAsync(string userId, int mediaId, int position, int duration);
        Task<List<WatchProgress>> GetContinueWatchingAsync(string userId, int limit, int offset);
    }

    public class PlaybackHistoryRepository : IPlaybackHistoryRepository
    {
        private readonly MediaContext _context;

        public PlaybackHistoryRepository(MediaContext context)
        {
            _context = context;
        }

        public async Task<List<History>> GetPlaybackHistoryAsync(string userId, int limit, int offset, string? mediaType = null)
        {
            var query = _context.History
                .Include(h => h.MediaItem)
                .Where(h => h.UserId == userId);

            if (!string.IsNullOrEmpty(mediaType))
            {
                query = query.Where(h => h.MediaItem.Type == mediaType);
            }

            return await query
                .OrderByDescending(h => h.WatchedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<History?> GetMoviePlaybackHistoryAsync(string userId, int mediaId)
        {
            return await _context.History
                .Include(h => h.MediaItem)
                .FirstOrDefaultAsync(h => h.UserId == userId && h.MediaId == mediaId && h.MediaItem.Type == "movie");
        }

        public async Task<History?> GetSeriesPlaybackHistoryAsync(string userId, int mediaId)
        {
            return await _context.History
                .Include(h => h.MediaItem)
                .FirstOrDefaultAsync(h => h.UserId == userId && h.MediaId == mediaId && h.MediaItem.Type == "series");
        }

        public async Task<bool> ReportPlaybackProgressAsync(string userId, int mediaId, int position, int duration)
        {
            var existingProgress = await _context.WatchProgress
                .FirstOrDefaultAsync(wp => wp.UserId == userId && wp.MediaId == mediaId);

            if (existingProgress != null)
            {
                existingProgress.Position = position;
                existingProgress.Duration = duration;
                existingProgress.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.WatchProgress.Add(new WatchProgress
                {
                    UserId = userId,
                    MediaId = mediaId,
                    Position = position,
                    Duration = duration,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // 也添加到历史记录
            var existingHistory = await _context.History
                .FirstOrDefaultAsync(h => h.UserId == userId && h.MediaId == mediaId);

            if (existingHistory != null)
            {
                existingHistory.WatchedAt = DateTime.UtcNow;
                existingHistory.Position = position;
            }
            else
            {
                _context.History.Add(new History
                {
                    UserId = userId,
                    MediaId = mediaId,
                    WatchedAt = DateTime.UtcNow,
                    Position = position
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<WatchProgress>> GetContinueWatchingAsync(string userId, int limit, int offset)
        {
            return await _context.WatchProgress
                .Include(wp => wp.MediaItem)
                .Where(wp => wp.UserId == userId && wp.Position > 0 && wp.Position < wp.Duration * 0.9) // 未完全观看的内容
                .OrderByDescending(wp => wp.UpdatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
    }
}
