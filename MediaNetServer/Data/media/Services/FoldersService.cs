using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class FoldersService
    {
        private readonly MediaContext _context;

        public FoldersService(MediaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Folders>> GetAllAsync()
        {
            return await _context.Folders
                .Include(f => f.Items)
                .ToListAsync();
        }

        public async Task<Folders> GetByIdAsync(Guid id)
        {
            return await _context.Folders
                .Include(f => f.Items)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Folders> CreateAsync(Folders folder)
        {
            folder.Id = Guid.NewGuid();
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }

        public async Task<bool> UpdateAsync(Folders folder)
        {
            var existing = await _context.Folders.FindAsync(folder.Id);
            if (existing == null) return false;

            existing.Name = folder.Name;
            existing.Path = folder.Path;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.Folders.FindAsync(id);
            if (existing == null) return false;

            _context.Folders.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
