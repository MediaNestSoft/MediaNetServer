using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class GenreService
    {
        private readonly MediaContext _context;

        public GenreService(MediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre> GetByIdAsync(int gid)
        {
            return await _context.Genres.FindAsync(gid);
        }

        public async Task<Genre> CreateAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return genre;
        }

        public async Task<bool> UpdateAsync(int gid, Genre genre)
        {
            var existing = await _context.Genres.FindAsync(gid);
            if (existing == null) return false;

            existing.genreId = genre.genreId;
            existing.Name = genre.Name;

            _context.Genres.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int gid)
        {
            var genre = await _context.Genres.FindAsync(gid);
            if (genre == null) return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
