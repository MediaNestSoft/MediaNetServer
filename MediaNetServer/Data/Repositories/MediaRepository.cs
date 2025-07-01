using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.Repositories
{
    public interface IMediaRepository
    {
        Task<MediaItem?> GetMediaByIdAsync(int id);
        Task<List<MediaItem>> GetMediaByGenreAsync(string genreId, int limit, int offset);
        Task<List<MediaItem>> SearchMediaByTitleAsync(string query, int limit, int offset);
        Task<List<Genre>> GetAllGenresAsync();
        Task<MovieDetail?> GetMovieDetailAsync(int mediaId);
        Task<SeriesDetail?> GetSeriesDetailAsync(int mediaId);
        Task<List<Season>> GetSeasonsAsync(int seriesId);
        Task<List<Episodes>> GetEpisodesAsync(int seriesId, int seasonNumber);
        Task<List<MediaCast>> GetMediaCreditsAsync(int mediaId);
        Task<List<Files>> GetMediaFilesAsync(int mediaId);
        Task<List<Images>> GetMediaImagesAsync(int mediaId);
    }

    public class MediaRepository : IMediaRepository
    {
        private readonly MediaContext _context;

        public MediaRepository(MediaContext context)
        {
            _context = context;
        }

        public async Task<MediaItem?> GetMediaByIdAsync(int id)
        {
            return await _context.MediaItems
                .Include(m => m.MovieDetails)
                .Include(m => m.SeriesDetails)
                .Include(m => m.MediaGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<MediaItem>> GetMediaByGenreAsync(string genreId, int limit, int offset)
        {
            return await _context.MediaItems
                .Include(m => m.MediaGenres)
                    .ThenInclude(mg => mg.Genre)
                .Where(m => m.MediaGenres.Any(mg => mg.Genre.Name == genreId))
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<MediaItem>> SearchMediaByTitleAsync(string query, int limit, int offset)
        {
            return await _context.MediaItems
                .Where(m => m.Title.Contains(query))
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<MovieDetail?> GetMovieDetailAsync(int mediaId)
        {
            return await _context.MovieDetails
                .FirstOrDefaultAsync(md => md.MediaId == mediaId);
        }

        public async Task<SeriesDetail?> GetSeriesDetailAsync(int mediaId)
        {
            return await _context.SeriesDetail
                .FirstOrDefaultAsync(sd => sd.MediaId == mediaId);
        }

        public async Task<List<Season>> GetSeasonsAsync(int seriesId)
        {
            return await _context.Seasons
                .Where(s => s.MediaId == seriesId)
                .OrderBy(s => s.SeasonNumber)
                .ToListAsync();
        }

        public async Task<List<Episodes>> GetEpisodesAsync(int seriesId, int seasonNumber)
        {
            return await _context.Episodes
                .Include(e => e.Season)
                .Where(e => e.Season.MediaId == seriesId && e.Season.SeasonNumber == seasonNumber)
                .OrderBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        public async Task<List<MediaCast>> GetMediaCreditsAsync(int mediaId)
        {
            return await _context.MediaCasts
                .Where(mc => mc.MediaId == mediaId)
                .ToListAsync();
        }

        public async Task<List<Files>> GetMediaFilesAsync(int mediaId)
        {
            return await _context.Files
                .Where(f => f.MediaId == mediaId)
                .ToListAsync();
        }

        public async Task<List<Images>> GetMediaImagesAsync(int mediaId)
        {
            return await _context.Images
                .Where(i => i.MediaId == mediaId)
                .ToListAsync();
        }
    }
}
