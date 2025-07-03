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

        public async Task<MovieDetail> GetByMediaIdAsync(int tmdbId)
        {
            return await _context.MovieDetails
                                 .Include(md => md.MediaItem)
                                 .FirstOrDefaultAsync(md => md.MediaItem.TMDbId == tmdbId);
        }

        public async Task<double> GetMovieDurationAsync(int tmdbId)
        {
            return await _context.MovieDetails
                                    .Where(md => md.MediaItem.TMDbId == tmdbId)
                                    .Select(md => md.Duration)
                                    .FirstOrDefaultAsync();
        }
        
        

        public async Task CreateAsync(MovieDetail detail)
        {
            _context.MovieDetails.Add(detail);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int mediaId)
        {
            return await _context.MovieDetails.AnyAsync(md => md.MediaId == mediaId);
        }

        public async Task<bool> UpdateAsync(int mediaId, MovieDetail detail)
        {
            var existing = await _context.MovieDetails.FindAsync(mediaId);
            if (existing == null)
                return false;

            //只更新简单字段，不处理导航属性
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
