using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class HistoryService
    {
        private readonly MediaContext _context;

        public HistoryService(MediaContext context)
        {
            _context = context;
        }

        public async Task<List<History>> GetAllAsync()
        {
            return await _context.History.ToListAsync();
        }

        public async Task<History> GetByIdAsync(int id)
        {
            return await _context.History.FindAsync(id);
        }

        public async Task<History> CreateAsync(History history)
        {
            _context.History.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<History> UpdateAsync(int id, History updated)
        {
            var existing = await _context.History.FindAsync(id);
            if (existing == null) return null;

            // 可选：逐字段更新
            existing.UserId = updated.UserId;
            existing.mediaId = updated.mediaId;
            existing.watchedAt = updated.watchedAt;
            existing.position = updated.position;
            existing.duration = updated.duration;
            existing.seasonNumber = updated.seasonNumber;
            existing.episodeNumber = updated.episodeNumber;
            existing.isFinished = updated.isFinished;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.History.FindAsync(id);
            if (existing == null) return false;

            _context.History.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
