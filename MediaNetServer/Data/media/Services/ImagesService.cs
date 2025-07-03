using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class ImagesService
    {
        private readonly MediaContext _context;

        public ImagesService(MediaContext context)
        {
            _context = context;
        }

        // 获取所有 Images
        public async Task<List<Images>> GetAllAsync()
        {
            return await _context.Images.ToListAsync();
        }

        // 获取单个 Image
        public async Task<Images?> getImage(string path)
        {
            return await _context.Images
                .FirstOrDefaultAsync(i => i.filePath == path);
        }

        // 新增 Image
        public async Task AddAsync(Images image)
        {
            var exists = await _context.MediaItems.AnyAsync(x => x.TMDbId == image.tmdbId);
            if (!exists) throw new InvalidOperationException($"No MediaItem with MediaId {image.tmdbId}");
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }

        // 更新 Image
        public async Task<bool> UpdateAsync(Images image)
        {
            var existing = await _context.Images.FindAsync(image.imageId);
            if (existing == null) return false;

            // 更新字段
            existing.imageType = image.imageType;
            existing.tmdbId = image.tmdbId;
            existing.filePath = image.filePath;
            existing.episodeNumber = image.episodeNumber;

            await _context.SaveChangesAsync();
            return true;
        }

        // 删除 Image
        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return false;

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public class ImageSettings
        {
            public string CachePath { get; set; } = string.Empty;
        }
    }
}
