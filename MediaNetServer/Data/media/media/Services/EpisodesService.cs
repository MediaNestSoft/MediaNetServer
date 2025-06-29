using Microsoft.EntityFrameworkCore;
using Media.Data;
using Media.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Services
{
    public class EpisodesService
    {
        private readonly MediaContext _context;

        public EpisodesService(MediaContext context)
        {
            _context = context;
        }

        // 查询所有剧集
        public async Task<List<Episodes>> GetAllAsync()
        {
            return await _context.Episodes
                .AsNoTracking()
                .ToListAsync();
        }

        // 根据ID查询单集
        public async Task<Episodes?> GetByIdAsync(int epId)
        {
            return await _context.Episodes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.epId == epId);
        }

        // 新增剧集
        public async Task<Episodes> CreateAsync(Episodes episode)
        {
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();
            return episode;
        }

        // 更新剧集
        public async Task<bool> UpdateAsync(int epId, Episodes episode)
        {
            var exist = await _context.Episodes.FindAsync(epId);
            if (exist == null) return false;

            // 更新字段
            exist.mediaId = episode.mediaId;
            exist.SeasonId = episode.SeasonId;
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
    }
}
