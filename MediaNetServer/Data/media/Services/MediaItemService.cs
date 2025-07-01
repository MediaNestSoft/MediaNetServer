using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class MediaItemService
    {
        private readonly MediaContext _context;

        public MediaItemService(MediaContext context)
        {
            _context = context;
        }

        // 获取所有媒体项
        public async Task<List<MediaItem>> GetAllMediaItemsAsync()
        {
            return await _context.MediaItems.ToListAsync();
        }

        // 根据 ID 获取单个媒体项
        public async Task<MediaItem?> GetMediaItemByIdAsync(int tmdbId)
        {
            return await _context.MediaItems
                .FirstOrDefaultAsync(mi => mi.TMDbId == tmdbId);
        }
        
        /// <summary>
        /// 按 AddTime（文件最后修改时间）倒序分页返回媒体实体
        /// </summary>
        public async Task<List<MediaItem>> GetRecentlyAddedAsync(int limit, int offset)
        {
            int skip = offset * limit;
            return await _context.MediaItems
                .AsNoTracking()
                .OrderByDescending(mi => mi.AddTime)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();
        }

        // 创建媒体项
        public async Task<MediaItem> CreateMediaItemAsync(MediaItem mediaItem)
        {
            _context.MediaItems.Add(mediaItem);
            await _context.SaveChangesAsync();
            return mediaItem;
        }

        // 更新媒体项
        public async Task<bool> UpdateMediaItemAsync(int id, MediaItem updatedItem)
        {
            var existing = await _context.MediaItems.FindAsync(id);
            if (existing == null)
                return false;

            // 更新字段
            existing.TMDbId = updatedItem.TMDbId;
            existing.Title = updatedItem.Title;
            existing.Type = updatedItem.Type;
            existing.PosterPath = updatedItem.PosterPath;
            existing.BackdropPath = updatedItem.BackdropPath;
            existing.LocalPath = updatedItem.LocalPath;
            existing.Rating = updatedItem.Rating;
            existing.ReleaseDate = updatedItem.ReleaseDate;
            existing.Country = updatedItem.Country;

            await _context.SaveChangesAsync();
            return true;
        }

        // 删除媒体项
        public async Task<bool> DeleteMediaItemAsync(int id)
        {
            var existing = await _context.MediaItems.FindAsync(id);
            if (existing == null)
                return false;

            _context.MediaItems.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
