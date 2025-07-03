using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class SeriesDetailService
    {
        private readonly MediaContext _context;

        public SeriesDetailService(MediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeriesDetail>> GetAllAsync()
        {
            return await _context.SeriesDetail
                .Include(s => s.MediaItem)
                .ToListAsync();
        }

        public async Task<SeriesDetail> GetByIdAsync(int mediaId)
        {
            return await _context.SeriesDetail
                .Include(s => s.MediaItem)
                .FirstOrDefaultAsync(s => s.mediaId == mediaId);
        }

        public async Task CreateAsync(SeriesDetail detail, bool same)
        {
            if (!same)
                return;
            _context.SeriesDetail.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int mediaId, SeriesDetail detail)
        {
            var existing = await _context.SeriesDetail.FindAsync(mediaId);
            if (existing == null) return false;

            existing.firstAirDate = detail.firstAirDate;
            existing.numberOfSeasons = detail.numberOfSeasons;
            existing.numberOfEpisodes = detail.numberOfEpisodes;

            _context.SeriesDetail.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int mediaId)
        {
            var existing = await _context.SeriesDetail.FindAsync(mediaId);
            if (existing == null) return false;

            _context.SeriesDetail.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
