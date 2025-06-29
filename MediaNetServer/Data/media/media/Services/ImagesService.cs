using Media.Data;
using Media.Models;
using Microsoft.EntityFrameworkCore;

namespace Media.Services
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

        // 根据 id 获取单个 Image
        public async Task<Images?> GetByIdAsync(int id)
        {
            return await _context.Images.FindAsync(id);
        }

        // 新增 Image
        public async Task<Images> AddAsync(Images image)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        // 更新 Image
        public async Task<bool> UpdateAsync(Images image)
        {
            var existing = await _context.Images.FindAsync(image.imageId);
            if (existing == null) return false;

            // 更新字段
            existing.imageType = image.imageType;
            existing.mediaId = image.mediaId;
            existing.filePath = image.filePath;
            existing.size = image.size;
            existing.width = image.width;
            existing.height = image.height;
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
    }
}
