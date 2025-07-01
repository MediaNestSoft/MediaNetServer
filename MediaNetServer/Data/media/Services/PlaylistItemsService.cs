using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class PlaylistItemsService
    {
        private readonly MediaContext _context;

        public PlaylistItemsService(MediaContext context)
        {
            _context = context;
        }

        // 获取指定播放列表下的所有项目
        public async Task<List<PlaylistItems>> GetByPlaylistIdAsync(int playlistId)
        {
            return await _context.PlaylistItems
                .Where(item => item.playlistId == playlistId)
                .ToListAsync();
        }

        // 根据项目 ID 获取单个项目
        public async Task<PlaylistItems?> GetByIdAsync(int id)
        {
            return await _context.PlaylistItems.FindAsync(id);
        }

        // 新增项目
        public async Task<PlaylistItems> AddAsync(PlaylistItems item)
        {
            _context.PlaylistItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        // 更新项目
        public async Task<bool> UpdateAsync(PlaylistItems item)
        {
            var existing = await _context.PlaylistItems.FindAsync(item.playlistItemId);
            if (existing == null) return false;

            existing.playlistId = item.playlistId;
            existing.tmdbId = item.tmdbId;
            existing.addedAt = item.addedAt;
            existing.releaseDate = item.releaseDate;

            await _context.SaveChangesAsync();
            return true;
        }

        // 删除项目
        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.PlaylistItems.FindAsync(id);
            if (item == null) return false;

            _context.PlaylistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
