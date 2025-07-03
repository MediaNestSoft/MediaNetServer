using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class EpisodesService
    {
        private readonly MediaContext _context;

        public EpisodesService(MediaContext context)
        {
            _context = context;
        }

        // 查询所有剧集
        public async Task<List<Episodes>> GetEpisodesBySeriesAndSeasonAsync(int seriesId, int seasonNumber)
        {
            var episodes = await _context.Episodes
                // 把 MediaItem 和 Season 一起加载进来
                .Include(e => e.MediaItem)
                .Include(e => e.Season)
                .Where(e =>
                    e.MediaItem.TMDbId == seriesId && e.Season.SeasonNumber == seasonNumber
                )
                .ToListAsync();

            return episodes;
        }
        
        public async Task<double> GetEpisodeDurationAsync(int tmdbId)
        {
            return await _context.Episodes
                .Where(e => e.tmdbId == tmdbId)
                .Select(e => e.duration)
                .FirstOrDefaultAsync();
        }

        // 根据ID查询单集
        public async Task<Episodes> GetByEpisodeImdbIdAsync(int tmdbId)
        {
            return await _context.Episodes
                .Where(e => e.tmdbId == tmdbId)
                .Select(e => new Episodes
                    {
                        epId = e.epId,
                        mediaId = e.mediaId,
                        tmdbId = e.tmdbId,
                        airDate = e.airDate,
                        SeasonId = e.SeasonId,
                        seasonNumber = e.seasonNumber,
                        episodeNumber = e.episodeNumber,
                        episodeName = e.episodeName,
                        duration = e.duration,
                        overview = e.overview,
                        stillPath = e.stillPath,
                        rating = e.rating
                    }
                )
                .FirstOrDefaultAsync();
        }

        // 新增剧集
        public async Task CreateAsync(Episodes episode)
        {
            if(await _context.Episodes.AnyAsync(e => e.tmdbId == episode.tmdbId))
                return;
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int mediaId, int seasonId, int episodeNumber)
        {
            return await _context.Episodes
                .AnyAsync(e => e.mediaId == mediaId &&
                               e.SeasonId == seasonId &&
                               e.episodeNumber == episodeNumber);
        }

        // 更新剧集
        public async Task<bool> UpdateAsync(int epId, Episodes episode)
        {
            var exist = await _context.Episodes.FindAsync(epId);
            if (exist == null) return false;

            // 更新字段
            exist.episodeNumber = episode.episodeNumber;
            exist.episodeName = episode.episodeName;
            exist.duration = episode.duration;
            exist.overview = episode.overview;
            exist.stillPath = episode.stillPath;

            await _context.SaveChangesAsync();
            return true;
        }

        // 删除剧集
        public async Task<bool> DeleteAsync(int epId)
        {
            var exist = await _context.Episodes.FindAsync(epId);
            if (exist == null) return false;

            _context.Episodes.Remove(exist);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<Episodes> GetEpisode(int tmdbId, int seasonNumber, int episodeNumber)
        {
            var media = await _context.MediaItems
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.TMDbId == tmdbId);
            if (media == null)
            {
                return null;
            }

            var season = await _context.Seasons
                .AsNoTracking()
                .SingleOrDefaultAsync(s =>
                    s.MediaId      == media.MediaId &&
                    s.SeasonNumber == seasonNumber);
            if (season == null)
            {
                return null;
            }

            var episode = await _context.Episodes
                .AsNoTracking()
                .SingleOrDefaultAsync(e =>
                    e.SeasonId      == season.SeasonId &&
                    e.episodeNumber == episodeNumber);

            return episode;
        }
    }
}
