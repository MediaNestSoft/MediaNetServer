using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class WatchProgressService
    {
        private readonly MediaContext _context;

        public WatchProgressService(MediaContext context)
        {
            _context = context;
        }

        // 获取所有记录
        public async Task<IEnumerable<WatchProgress>> GetAllAsync()
        {
            return await _context.WatchProgress.ToListAsync();
        }

        // 获取单条记录
        public async Task<WatchProgress?> GetByIdAsync(int id)
        {
            return await _context.WatchProgress.FirstOrDefaultAsync(wp => wp.watchProgressId == id);
        }

        // 创建
        public async Task<WatchProgress> CreateAsync(WatchProgress watchProgress)
        {
            _context.WatchProgress.Add(watchProgress);
            await _context.SaveChangesAsync();
            return watchProgress;
        }

        // 更新
        public async Task<WatchProgress?> UpdateAsync(int id, WatchProgress updated)
        {
            var existing = await _context.WatchProgress.FindAsync(id);
            if (existing == null) return null;

            existing.UserId = updated.UserId;
            existing.mediaId = updated.mediaId;
            existing.lastWatched = updated.lastWatched;
            existing.position = updated.position;
            existing.seasonNumber = updated.seasonNumber;
            existing.episodeNumber = updated.episodeNumber;

            await _context.SaveChangesAsync();
            return existing;
        }

        // 删除
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.WatchProgress.FindAsync(id);
            if (existing == null) return false;

            _context.WatchProgress.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
