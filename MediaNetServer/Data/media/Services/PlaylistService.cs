using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class PlaylistService
    {
        private readonly MediaContext _context;

        public PlaylistService(MediaContext context)
        {
            _context = context;
        }

        // 获取所有播放列表
        public async Task<List<Playlist>> GetAllAsync()
        {
            return await _context.Playlists.ToListAsync();
        }

        // 根据 ID 获取单个播放列表
        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await _context.Playlists.FindAsync(id);
        }

        // 新增播放列表
        public async Task<Playlist> AddAsync(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        // 更新播放列表
        public async Task<bool> UpdateAsync(Playlist playlist)
        {
            var existing = await _context.Playlists.FindAsync(playlist.playlistId);
            if (existing == null) return false;

            existing.UserId = playlist.UserId;
            existing.name = playlist.name;
            existing.isSystem = playlist.isSystem;

            await _context.SaveChangesAsync();
            return true;
        }

        // 删除播放列表
        public async Task<bool> DeleteAsync(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null) return false;

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
