using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class MovieDetailService
    {
        private readonly MediaContext _context;

        public MovieDetailService(MediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieDetail>> GetAllAsync()
        {
            return await _context.MovieDetails
                                 .Include(md => md.MediaItem)
                                 .ToListAsync();
        }

        public async Task<MovieDetail> GetByMediaIdAsync(int mediaId)
        {
            return await _context.MovieDetails
                                 .Include(md => md.MediaItem)
                                 .FirstOrDefaultAsync(md => md.MediaId == mediaId);
        }

        public async Task<MovieDetail> CreateAsync(MovieDetail detail)
        {
            // ✅ 明确忽略客户端传入的 MediaItem 对象，防止级联写入
            detail.MediaItem = null;

            // ✅ 只允许 mediaId 是数据库中已存在的 MediaItem 主键
            var media = await _context.MediaItems.FindAsync(detail.MediaId);
            if (media == null)
                return null; // ❌ 不存在则不插入

            _context.MovieDetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<bool> UpdateAsync(int mediaId, MovieDetail detail)
        {
            var existing = await _context.MovieDetails.FindAsync(mediaId);
            if (existing == null)
                return false;

            // ✅ 只更新简单字段，不处理导航属性
            existing.Overview = detail.Overview;
            existing.Duration = detail.Duration;

            _context.MovieDetails.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int mediaId)
        {
            var existing = await _context.MovieDetails.FindAsync(mediaId);
            if (existing == null)
                return false;

            _context.MovieDetails.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
