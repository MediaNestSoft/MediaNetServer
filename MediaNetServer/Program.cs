using System.Net;
using MediaNetServer.Services.Folder;
using MediaNetServer.Services.MediaServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TMDbLib.Client;
using MediaNetServer.Data.media;
using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Services;
using MediaNetServer.Services.AuthorizationServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

const string tmdbApiKey = "fee09865c006d213b701f3aef5629d1e";
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
builder.Services.AddSingleton(sp =>
{
    var client = new TMDbClient(tmdbApiKey);
    client.GetConfigAsync().GetAwaiter().GetResult();
    return client;
});

builder.Services.AddScoped<UserAuthService>();
builder.Services.AddScoped<HistoryService>();
builder.Services.AddScoped<WatchProgressService>();
builder.Services.AddScoped<MediaItemService>();
builder.Services.AddScoped<MovieDetailService>();
builder.Services.AddScoped<EpisodesService>();
builder.Services.AddScoped<FolderScraperService>();
builder.Services.Configure<ImagesService.ImageSettings>(builder.Configuration.GetSection("ImageSettings"));

builder.Services.AddSingleton<ImageCacheService>(sp => {
    var settings = sp.GetRequiredService<IOptions<ImagesService.ImageSettings>>().Value;
    return new ImageCacheService(settings.CachePath);
});

DatabaseProgram.AddDatabaseServices(builder);
builder.Services.AddDbContext<MediaContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(6666);
});

var app = builder.Build();

Console.WriteLine("ConnStr = " + builder.Configuration.GetConnectionString("DefaultConnection"));

// 🔧 中间件配置顺序
app.UseRouting();

// 如果你有 HTTPS 可启用下面一行
// app.UseHttpsRedirection();

app.UseAuthentication();

// 👇 映射 API 控制器
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "image",
        pattern: "media/image/{*path}",
        defaults: new { controller = "Media", action = "ImageHandler" });
});

// 🛠 初始化数据库及刮削
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<MediaContext>();
    ctx.Database.Migrate();

    var auth = scope.ServiceProvider.GetRequiredService<UserAuthService>();
    auth.StartInteractive();

    var scraper = scope.ServiceProvider.GetRequiredService<FolderScraperService>();
    var folder = "/Volumes/T9ExFAT/Vedios";
    if (!Directory.Exists(folder))
    {
        Console.Error.WriteLine($"错误：找不到路径 {folder}");
        return;
    }
    await scraper.ScrapeFolderAsync(folder);
}

app.Run();