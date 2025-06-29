using Media.Data;
using Media.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Media.Services
{
    public class MediaCastService
    {
        private readonly MediaContext _context;

        // 构造函数，注入数据库上下文
        public MediaCastService(MediaContext context)
        {
            _context = context;
        }

        // 获取所有 MediaCast（即演员信息）
        public async Task<IEnumerable<MediaCast>> GetAllAsync()
        {
            // 使用 EF Core 的异步查询，获取所有 MediaCast 数据并返回
            return await _context.MediaCasts.ToListAsync();
        }

        // 根据 personId 获取单个 MediaCast
        public async Task<MediaCast?> GetByPersonIdAsync(int personId)
        {
            // 根据主键 personId 查找并返回 MediaCast
            return await _context.MediaCasts.FindAsync(personId);
        }

        // 创建新的 MediaCast
        public async Task<MediaCast?> CreateAsync(MediaCast cast)
        {
            // 确保提供的 mediaId 对应的 MediaItem 已存在
            var mediaItem = await _context.MediaItems.FindAsync(cast.MediaId);
            if (mediaItem == null)
                return null; // 如果 MediaItem 不存在，则不创建 MediaCast

            // 将新的 MediaCast 添加到数据库
            _context.MediaCasts.Add(cast);
            await _context.SaveChangesAsync(); // 异步保存更改
            return cast; // 返回创建的 MediaCast 对象
        }

        // 更新现有的 MediaCast
        public async Task<bool> UpdateAsync(int personId, MediaCast cast)
        {
            // 查找已有的 MediaCast
            var existing = await _context.MediaCasts.FindAsync(personId);
            if (existing == null) return false; // 如果没有找到，返回 false

            // 更新字段，只修改 Overview、Department 和 PersonUrl
            existing.Name = cast.Name;
            existing.Department = cast.Department;
            existing.PersonUrl = cast.PersonUrl;

            // 更新数据库中的 MediaCast 数据
            _context.MediaCasts.Update(existing);
            await _context.SaveChangesAsync();
            return true; // 返回更新成功
        }

        // 删除指定的 MediaCast
        public async Task<bool> DeleteAsync(int personId)
        {
            // 查找要删除的 MediaCast
            var cast = await _context.MediaCasts.FindAsync(personId);
            if (cast == null) return false; // 如果没有找到，返回 false

            // 从数据库中删除 MediaCast
            _context.MediaCasts.Remove(cast);
            await _context.SaveChangesAsync(); // 异步保存更改
            return true; // 返回删除成功
        }
    }
}
