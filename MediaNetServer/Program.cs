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
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(6666);
});

builder.Services.AddSingleton(sp =>
{
    var client = new TMDbClient(tmdbApiKey)
    {
        //DefaultLanguage      = "zh-CN",
        //DefaultImageLanguage = "en,null"
    };
    client.GetConfigAsync()
        .GetAwaiter()
        .GetResult();
    return client;
});

// 注册其他服务
builder.Services.AddScoped<UserAuthService>();
builder.Services.AddScoped<HistoryService>();
builder.Services.AddScoped<WatchProgressService>();
builder.Services.AddScoped<MediaItemService>();
builder.Services.AddScoped<MovieDetailService>();
builder.Services.AddScoped<EpisodesService>();
builder.Services.AddScoped<FolderScraperService>();
builder.Services.Configure<ImagesService.ImageSettings>(builder.Configuration.GetSection("ImageSettings"));

builder.Services.AddControllers();

var folder = "/Volumes/T9ExFAT/Vedios";
if (!Directory.Exists(folder))
{
    Console.Error.WriteLine($"错误：找不到路径 {folder}");
    return;
}

DatabaseProgram.AddDatabaseServices(builder);
builder.Services.AddDbContext<MediaContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<ImageCacheService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ImagesService.ImageSettings>>().Value;
    return new ImageCacheService(settings.CachePath);
});
var app = builder.Build();

Console.WriteLine("ConnStr = " + builder.Configuration.GetConnectionString("DefaultConnection"));

app.UseRouting();
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "media/image/{*path}");
});

// 启动时执行一次刮削并持久化
using (var scope = app.Services.CreateScope())
{
    // 确保数据库迁移
    var ctx = scope.ServiceProvider.GetRequiredService<MediaContext>();
    ctx.Database.Migrate();
    // 登录注册
    var auth = scope.ServiceProvider.GetRequiredService<UserAuthService>();
    auth.StartInteractive();
    var mediaService = scope.ServiceProvider.GetRequiredService<FolderScraperService>();
    // 刮削
    await mediaService.ScrapeFolderAsync(folder);
}

app.Run();