using Media.Data;
using Media.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Services
{
    public class MediaGenresService
    {
        private readonly MediaContext _context;

        public MediaGenresService(MediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MediaGenres>> GetAllAsync()
        {
            return await _context.MediaGenres
                .Include(mg => mg.MediaItem)
                .Include(mg => mg.Genre)
                .ToListAsync();
        }

        public async Task<MediaGenres> GetByIdAsync(int id)
        {
            return await _context.MediaGenres
                .Include(mg => mg.MediaItem)
                .Include(mg => mg.Genre)
                .FirstOrDefaultAsync(mg => mg.mediaGenreId == id);
        }

        public async Task<MediaGenres> CreateAsync(MediaGenres mg)
        {
            _context.MediaGenres.Add(mg);
            await _context.SaveChangesAsync();
            return mg;
        }

        public async Task<bool> UpdateAsync(int id, MediaGenres mg)
        {
            var existing = await _context.MediaGenres.FindAsync(id);
            if (existing == null) return false;

            existing.mediaId = mg.mediaId;
            existing.genreId = mg.genreId;

            _context.MediaGenres.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mg = await _context.MediaGenres.FindAsync(id);
            if (mg == null) return false;

            _context.MediaGenres.Remove(mg);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
