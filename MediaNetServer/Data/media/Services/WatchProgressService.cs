using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using MediaNetServer.Services.MediaServices;
using Microsoft.EntityFrameworkCore;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace MediaNetServer.Data.media.Services
{
    public class WatchProgressService
    {
        private readonly MediaContext _context;
        private readonly MediaItemService _mediaSvc;
        private readonly EpisodesService _episodesSvc;
        private readonly MovieDetailService _movieDetailSvc;

        public WatchProgressService(MediaContext context, MediaItemService mediaSvc, EpisodesService episodesSvc,
            MovieDetailService movieDetailSvc)
        {
            _context = context;
            _mediaSvc = mediaSvc;
            _episodesSvc = episodesSvc;
            _movieDetailSvc = movieDetailSvc;
        }

        // 获取所有记录
        public async Task<IEnumerable<WatchProgress>> GetAllAsync()
        {
            return await _context.WatchProgress.ToListAsync();
        }

        // 获取单条记录
        public async Task<WatchProgress?> GetByIdAsync(int id)
        {
            return await _context.WatchProgress.FirstOrDefaultAsync(wp => wp.watchProgressId == id);
        }

        /// <summary>
        /// 如果已存在该用户该媒体的进度，则更新；否则插入新纪录
        /// </summary>
        public async Task UpdateOrCreateAsync(WatchProgress progress, CancellationToken ct = default)
        {
            var existing = await _context.WatchProgresses
                .FirstOrDefaultAsync(wp =>
                    wp.UserId == progress.UserId &&
                    wp.tmdbId == progress.tmdbId, ct)
                .ConfigureAwait(false);

            if (existing != null)
            {
                // 更新字段
                existing.position      = progress.position;
                existing.lastWatched   = progress.lastWatched;
                existing.seasonNumber  = progress.seasonNumber;
                existing.episodeNumber = progress.episodeNumber;
            }
            else
            {
                // 新增记录
                await _context.WatchProgresses
                    .AddAsync(progress, ct)
                    .ConfigureAwait(false);
            }

            await _context.SaveChangesAsync(ct)
                .ConfigureAwait(false);
        }

        // 删除
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.WatchProgress.FindAsync(id);
            if (existing == null) return false;

            _context.WatchProgress.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 获取用户的“继续观看”列表，按 UpdatedAt 倒序，分页返回
        /// </summary>
        public async Task<List<ContinueWatchItem>> GetContinueWatchingAsync(
            string userId, int limit, int offset)
        {
            // 拉出用户观看进度
            var progresses = await _context.WatchProgresses
                .AsNoTracking()
                .Where(w => w.UserId.ToString() == userId)
                .OrderByDescending(w => w.lastWatched)
                .Skip(offset * limit).Take(limit)
                .ToListAsync();
            
            var result = new List<ContinueWatchItem>();
        foreach (var wp in progresses)
        {
            // 2. 拿到媒体基础信息
            var media = await _mediaSvc.GetMediaItemByIdAsync(wp.tmdbId);
            if (media == null) continue;

            // 3. 区分电影／电视剧
            if (media.Type.Equals("Movie", StringComparison.OrdinalIgnoreCase))
            {
                double duration = await _movieDetailSvc.GetMovieDurationAsync(media.TMDbId);
                // 电影：additional 填上映年份，season/episode 置空
                result.Add(new ContinueWatchItem(
                    mediaId:     new Option<int?>(media.MediaId),
                    title:       new Option<string?>(media.Title),
                    type:        new Option<ContinueWatchItem.TypeEnum?>(ContinueWatchItem.TypeEnum.Movie),
                    posterPath:  new Option<string?>(media.PosterPath),
                    additional:  new Option<string?>(media.ReleaseDate.Year.ToString()),
                    seasonNumber:new Option<int?>(null),
                    episodeNumber:new Option<int?>(null),
                    position:    new Option<int?>(wp.position),
                    runtime:     new Option<int?>((int)duration)
                ));
            }
            else
            {
                // 电视剧：season/episode 从 Episodes 表里查
                var ep = await _episodesSvc.GetByEpisodeImdbIdAsync(wp.tmdbId);
                result.Add(new ContinueWatchItem(
                    mediaId:      new Option<int?>(media.MediaId),
                    title:        new Option<string?>(media.Title),
                    type:         new Option<ContinueWatchItem.TypeEnum?>(ContinueWatchItem.TypeEnum.Series),
                    posterPath:   new Option<string?>(media.PosterPath),
                    additional:   new Option<string?>(null),
                    seasonNumber: new Option<int?>(ep?.seasonNumber),
                    episodeNumber:new Option<int?>(ep?.episodeNumber),
                    position:     new Option<int?>(wp.position),
                    runtime:      new Option<int?>(ep.duration)
                ));
            }
        }
        return result;
        }
    }
}
