using MediaNetServer.Services.Folder;
using MediaNetServer.Services.MediaServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TMDbLib.Client;
using MediaNetServer.Data.media;
using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Services;
using MediaNetServer.Services.AuthorizationServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

// 注册其他服务
builder.Services.AddSingleton<FolderScraperService>();
builder.Services.AddScoped<MediaDetailService>();
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

var app = builder.Build();

Console.WriteLine("ConnStr = " + builder.Configuration.GetConnectionString("DefaultConnection"));

app.UseRouting();
app.MapControllers();

// 启动时执行一次刮削并持久化
using (var scope = app.Services.CreateScope())
{
    // 登录注册
    var auth = scope.ServiceProvider.GetRequiredService<UserAuthService>();
    auth.StartInteractive();
    // 确保数据库迁移
    var ctx = scope.ServiceProvider.GetRequiredService<MediaContext>();
    ctx.Database.Migrate();
    var mediaService = scope.ServiceProvider.GetRequiredService<MediaDetailService>();
    // 刮削
    await mediaService.ProcessMediaAsync(folder);
}

app.Run();