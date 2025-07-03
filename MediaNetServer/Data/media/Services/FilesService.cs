using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaNetServer.Data.media.Services;

public class FilesService
{
    private readonly MediaContext _context;

    public FilesService(MediaContext context)
    {
        _context = context;
    }

    // 查询 Files
    public async Task<Files> GetFileByFid(string fid)
    {
        return await _context.Files
            .Include(f => f.MediaItem)
            .FirstOrDefaultAsync(f=> f.fid.ToString() == fid);
    }

    // 根据 id 查询单个 Files
    public async Task<List<Files>> GetByIdAsync(int tmdbId)
    {
        return await _context.Files
            .Include(f => f.MediaItem)
            .Where(f => f.tmdbId == tmdbId)
            .ToListAsync();
    }

    // 新增 Files
    public async Task<Files> CreateAsync(Files file)
    {
        _context.Files.Add(file);
        await _context.SaveChangesAsync();
        return file;
    }

    // 更新 Files
    public async Task<bool> UpdateAsync(int id, Files file)
    {
        var existFile = await _context.Files.FindAsync(id);
        if (existFile == null) return false;

        // 更新字段
        existFile.fileId = file.fileId;
        existFile.tmdbId = file.tmdbId;
        //existFile.playhistory = file.playhistory;
        existFile.filePath = file.filePath;

        await _context.SaveChangesAsync();
        return true;
    }

    // 删除 Files
    public async Task<bool> DeleteAsync(int id)
    {
        var existFile = await _context.Files.FindAsync(id);
        if (existFile == null) return false;

        _context.Files.Remove(existFile);
        await _context.SaveChangesAsync();
        return true;
    }
}