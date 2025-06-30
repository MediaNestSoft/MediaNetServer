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

    // 查询所有 Files
    public async Task<List<Files>> GetAllAsync()
    {
        return await _context.Files
            .Include(f => f.MediaItem) // 如果需要关联查询MediaItem
            .ToListAsync();
    }

    // 根据 id 查询单个 Files
    public async Task<Files> GetByIdAsync(int id)
    {
        return await _context.Files
            .Include(f => f.MediaItem)
            .FirstOrDefaultAsync(f => f.fid == id);
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
        existFile.mediaId = file.mediaId;
        existFile.playhistory = file.playhistory;
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