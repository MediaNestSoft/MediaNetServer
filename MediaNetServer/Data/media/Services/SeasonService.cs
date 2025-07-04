﻿using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services
{
    public class SeasonService
    {
        private readonly MediaContext _context;

        public SeasonService(MediaContext context)
        {
            _context = context;
        }
        

        public async Task<List<Season>> GetSeasonsByMediaIdAsync(int seriesId)
        {
            return await _context.Seasons
                .Include(s => s.MediaItem)
                .Where(s => s.MediaItem.TMDbId == seriesId)
                .OrderBy(s => s.SeasonNumber)
                .ToListAsync();
        }

        public async Task<Season> CreateOrGetSeasonAsync(int mediaId, int seasonNumber, DateTime firstAirdate)
        {
            var existing = await _context.Seasons
                .FirstOrDefaultAsync(s => s.MediaId == mediaId && s.SeasonNumber == seasonNumber);
            if (existing != null)
                return existing;
            var newSeason = new Season
            {
                MediaId = mediaId,
                SeasonNumber = seasonNumber,
                AirDate = firstAirdate
            };
            _context.Seasons.Add(newSeason);
            await _context.SaveChangesAsync();
            return newSeason;
        }

        public async Task<Season?> GetSeasonByIdAsync(int seasonId)
        {
            return await _context.Seasons.FindAsync(seasonId);
        }

        public async Task<bool> AddSeasonAsync(Season season)
        {
            if(await _context.Seasons
                   .AnyAsync(s => s.SeasonId == season.SeasonId))
                return true;
            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();
            return false;
        }
        public async Task<bool> ExistsAsync(int mediaId, int seasonNumber)
        {
            return await _context.Seasons
                .AnyAsync(s => s.MediaId == mediaId && s.SeasonNumber == seasonNumber);
        }

        public async Task<Season?> UpdateSeasonAsync(int seasonId, Season season)
        {
            var existing = await _context.Seasons.FindAsync(seasonId);
            if (existing == null) return null;

            existing.MediaId = season.MediaId;
            existing.SeasonNumber = season.SeasonNumber;
            existing.SeasonName = season.SeasonName;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteSeasonAsync(int seasonId)
        {
            var season = await _context.Seasons.FindAsync(seasonId);
            if (season == null) return false;

            _context.Seasons.Remove(season);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
