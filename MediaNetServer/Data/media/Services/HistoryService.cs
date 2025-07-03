using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class HistoryService
    {
        private readonly MediaContext _context;
        private readonly MediaItemService _mediaItemService;
        private readonly MovieDetailService _movieDetailService;
        private readonly EpisodesService _episodesService;

        public HistoryService(MediaContext context, MediaItemService mediaItemService,
            MovieDetailService movieDetailService, EpisodesService episodesService)
        {
            _context = context;
            _mediaItemService = mediaItemService;
            _movieDetailService = movieDetailService;
            _episodesService = episodesService;
        }

        public async Task<List<History>> GetAllHistoryByUserIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                return new List<History>();

            var histories = await _context.History
                .AsNoTracking()  
                .Include(h => h.MediaItem)
                .Where(h => h.UserId == userGuid)
                .ToListAsync();

            return histories;
        }

        public async Task<List<History>> GetHistoryByUserIdAsync(string userId, int tmdbId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return new List<History>();
            }
            var histories = await _context.History
                .AsNoTracking()
                .Include(h => h.MediaItem)
                .Where(h => 
                    h.UserId == userGuid && 
                    h.tmdbId  == tmdbId
                )
                .ToListAsync();

            return histories;
        }

        /// <summary>
        /// 如果已存在该用户该媒体的历史记录，则更新对应字段；否则插入新纪录
        /// </summary>
        public async Task UpdateOrCreateHistoryAsync(History history, CancellationToken ct = default)
        {
            if (history.duration <= 0)
            {
                var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(history.tmdbId);
                if (mediaItem != null)
                {
                    if (mediaItem.Type.Equals("movie", StringComparison.OrdinalIgnoreCase))
                    {
                        var movieDetail = await _movieDetailService.GetByMediaIdAsync(history.tmdbId);
                        history.duration = movieDetail?.Duration * 60 ?? 0;
                    }
                    else if (mediaItem.Type.Equals("series", StringComparison.OrdinalIgnoreCase))
                    {
                        var season = history.seasonNumber ?? -1;
                        var episode = history.episodeNumber ?? -1;
                        var ep = await _episodesService.GetEpisode(history.tmdbId, season, episode);
                        history.duration = ep?.duration * 60 ?? 0;
                    }
                }
            }
            
            var existing = await _context.History
                .FirstOrDefaultAsync(h =>
                        h.UserId == history.UserId &&
                        h.tmdbId == history.tmdbId,
                    ct)
                .ConfigureAwait(false);

            if (existing != null)
            {
                existing.position      = history.position;
                existing.watchedAt     = history.watchedAt;
                existing.duration      = history.duration;
                existing.seasonNumber  = history.seasonNumber;
                existing.episodeNumber = history.episodeNumber;
                existing.isFinished    = history.isFinished;
            }
            else
            {
                await _context.History.AddAsync(history, ct)
                    .ConfigureAwait(false);
            }

            await _context.SaveChangesAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.History.FindAsync(id);
            if (existing == null) return false;

            _context.History.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
