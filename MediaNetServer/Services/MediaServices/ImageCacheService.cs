using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MediaNetServer.Services.MediaServices;

public class ImageCacheService
{
    private readonly string _cachePath;
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string TmdbImageBaseUrl = "https://image.tmdb.org/t/p/original";

    public ImageCacheService(string cachePath)
    {
        _cachePath = cachePath;
    }
    public async Task CacheImageAsync(string? tmdbPath)
    {
        if (string.IsNullOrEmpty(tmdbPath))
            return;

        // 去掉前导斜杠，取文件名
        var fileName = Path.GetFileName(tmdbPath);
        if (string.IsNullOrEmpty(fileName))
            return;

        // 本地目标路径
        var localPath = Path.Combine(_cachePath, fileName);

        // 已经缓存过则直接返回
        if (File.Exists(localPath))
            return;

        // 确保目录存在
        System.IO.Directory.CreateDirectory(_cachePath);

        // 拼完整 URL 并下载
        var url = $"{TmdbImageBaseUrl}{tmdbPath}";
        var bytes = await _httpClient.GetByteArrayAsync(url);

        // 写入本地
        await File.WriteAllBytesAsync(localPath, bytes);
    }
    
}