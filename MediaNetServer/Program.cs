using MediaNetServer.Services.Folder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TMDbLib.Client;

var builder = WebApplication.CreateBuilder(args);

const string tmdbApiKey = "fee09865c006d213b701f3aef5629d1e";

builder.Services.AddSingleton(sp =>
{
    var client = new TMDbClient(tmdbApiKey)
    {
        DefaultLanguage      = "zh-CN",
        DefaultImageLanguage = "zh-CN"
    };
    return client;
});

// 注册你的 FolderScraperService
builder.Services.AddSingleton<FolderScraperService>();

builder.Services.AddControllers();

var app = builder.Build();
var scraper = app.Services.GetRequiredService<FolderScraperService>();
var folder = "/Volumes/T9ExFAT/Vedios";
if (!Directory.Exists(folder))
{
    Console.Error.WriteLine($"错误：找不到路径 {folder}");
    return;
}
await scraper.ScrapeFolderAsync(folder);
Console.WriteLine("刮削完成，程序退出。");