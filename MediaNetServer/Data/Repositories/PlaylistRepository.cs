using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.Repositories
{
    public interface IPlaylistRepository
    {
        Task<List<Playlist>> GetPlaylistsByUserIdAsync(string userId);
        Task<Playlist?> GetPlaylistByIdAsync(int playlistId);
        Task<Playlist> CreatePlaylistAsync(string userId, string name, string? description = null);
        Task<bool> RenamePlaylistAsync(int playlistId, string newName);
        Task<bool> DeletePlaylistAsync(int playlistId);
        Task<bool> AddMediaToPlaylistAsync(int playlistId, List<int> mediaIds);
        Task<bool> RemoveMediaFromPlaylistAsync(int playlistId, List<int> mediaIds);
        Task<List<PlaylistItems>> GetPlaylistItemsAsync(int playlistId, int limit, int offset);
        Task<bool> ClearPlaylistAsync(int playlistId);
    }

    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly MediaContext _context;

        public PlaylistRepository(MediaContext context)
        {
            _context = context;
        }

        public async Task<List<Playlist>> GetPlaylistsByUserIdAsync(string userId)
        {
            return await _context.Playlists
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Playlist?> GetPlaylistByIdAsync(int playlistId)
        {
            return await _context.Playlists
                .Include(p => p.PlaylistItems)
                    .ThenInclude(pi => pi.MediaItem)
                .FirstOrDefaultAsync(p => p.Id == playlistId);
        }

        public async Task<Playlist> CreatePlaylistAsync(string userId, string name, string? description = null)
        {
            var playlist = new Playlist
            {
                UserId = userId,
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public async Task<bool> RenamePlaylistAsync(int playlistId, string newName)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null) return false;

            playlist.Name = newName;
            playlist.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlaylistAsync(int playlistId)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null) return false;

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMediaToPlaylistAsync(int playlistId, List<int> mediaIds)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null) return false;

            var existingItems = await _context.PlaylistItems
                .Where(pi => pi.PlaylistId == playlistId)
                .Select(pi => pi.MediaId)
                .ToListAsync();

            var newMediaIds = mediaIds.Except(existingItems).ToList();
            
            foreach (var mediaId in newMediaIds)
            {
                _context.PlaylistItems.Add(new PlaylistItems
                {
                    PlaylistId = playlistId,
                    MediaId = mediaId,
                    AddedAt = DateTime.UtcNow
                });
            }

            playlist.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMediaFromPlaylistAsync(int playlistId, List<int> mediaIds)
        {
            var itemsToRemove = await _context.PlaylistItems
                .Where(pi => pi.PlaylistId == playlistId && mediaIds.Contains(pi.MediaId))
                .ToListAsync();

            if (!itemsToRemove.Any()) return false;

            _context.PlaylistItems.RemoveRange(itemsToRemove);

            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist != null)
            {
                playlist.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PlaylistItems>> GetPlaylistItemsAsync(int playlistId, int limit, int offset)
        {
            return await _context.PlaylistItems
                .Include(pi => pi.MediaItem)
                .Where(pi => pi.PlaylistId == playlistId)
                .OrderBy(pi => pi.AddedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> ClearPlaylistAsync(int playlistId)
        {
            var itemsToRemove = await _context.PlaylistItems
                .Where(pi => pi.PlaylistId == playlistId)
                .ToListAsync();

            if (!itemsToRemove.Any()) return false;

            _context.PlaylistItems.RemoveRange(itemsToRemove);

            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist != null)
            {
                playlist.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
