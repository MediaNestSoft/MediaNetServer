using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class PlaylistService
    {
        private readonly MediaContext _context;
        private const string FavoritesName     = "favorites";
        private const string ContinueWatchName = "continuewatch";

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
        public async Task<Playlist?> GetByIdAsync(int playlistId)
        {
            return await _context.Playlists
                .FirstOrDefaultAsync(p => p.playlistId == playlistId);
        }
        
        /// <summary>
        /// 判断某用户的某个“系统列表”（favorites / continuewatch）里，是否包含这条媒体
        /// </summary>
        private async Task<bool> IsInSystemListAsync(string userId, string listName, int mediaId)
        {
            var list = await _context.Playlists
                .SingleOrDefaultAsync(p =>
                    p.UserId.ToString()   == userId && 
                    p.isSystem == true &&
                    p.name     == listName);

            if (list == null)
                return false;

            // 去 PlaylistItems 表里查有没有这条 media
            return await _context.PlaylistItems
                .AsNoTracking()
                .AnyAsync(pi =>
                    pi.playlistId  == list.pId &&
                    pi.tmdbId  == mediaId);
        }

        public Task<bool> IsFavoriteAsync(string userId, int mediaId) =>
            IsInSystemListAsync(userId, FavoritesName, mediaId);

        public Task<bool> IsInContinueWatchAsync(string userId, int mediaId) =>
            IsInSystemListAsync(userId, ContinueWatchName, mediaId);

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
            var existing = await _context.Playlists.FindAsync(playlist.pId);
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
