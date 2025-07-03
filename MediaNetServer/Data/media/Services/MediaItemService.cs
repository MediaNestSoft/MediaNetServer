using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;
using Org.OpenAPITools.Client;

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
        public async Task<MediaItem> GetMediaItemsAsync(int mediaId)
        {
            return await _context.MediaItems
                .AsNoTracking()
                .FirstOrDefaultAsync(mi => mi.MediaId == mediaId);
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
        public async Task<List<MediaItem>> GetRecentlyAddedAsync()
        {
            return await _context.MediaItems
                .AsNoTracking()
                .OrderByDescending(mi => mi.AddTime)
                .ToListAsync();
        }

        // 创建媒体项
        public async Task<bool> CreateMediaItemAsync(MediaItem mediaItem)
        {
            bool exists = await _context.MediaItems
                .AnyAsync(mi => mi.TMDbId == mediaItem.TMDbId);
            if (exists)
            {
                return true;
            }

            _context.MediaItems.Add(mediaItem);
            await _context.SaveChangesAsync();
            return false;
        }
        
        public async Task<List<MediaItem>> SearchByTitleAsync(string q)
        {
            var query = _context.MediaItems
                .AsNoTracking()
                .Where(m => m.Title.StartsWith(q));

            var result = new List<MediaItem>();
            foreach (var m in query)
            {
                var mediaItem = new MediaItem
                {
                    TMDbId = m.TMDbId,
                    Title = m.Title,
                    Type = m.Type,
                    PosterPath = m.PosterPath
                };

                result.Add(mediaItem);
            }

            return result;
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
        
        /// <summary>
        /// 获取所有媒体项，可按类型过滤
        /// </summary>
        /// <param name="type"></param>
        public async Task<List<MediaItem>> GetMediaListAsync(string? type = null)
        {
            var query = _context.MediaItems.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query
                    .Where(m => m.Type.ToLower() == type.ToLower());
            }

            return await query.ToListAsync();
        }
    }
}
