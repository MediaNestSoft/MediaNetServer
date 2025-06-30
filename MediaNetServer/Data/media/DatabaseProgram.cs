using System.Text;
using MediaNetServer.Data.media.Data;
using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MediaNetServer.Data.media
{
    public static class DatabaseProgram
    {
        /// <summary>
        /// 在主程序中调用此方法以注册数据库和相关服务
        /// </summary>
        public static void AddDatabaseServices(WebApplicationBuilder builder)
        {
            // 配置数据库连接
            builder.Services.AddDbContext<MediaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 注册业务逻辑服务
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<MediaItemService>();
            builder.Services.AddScoped<MovieDetailService>();
            builder.Services.AddScoped<MediaCastService>();
            builder.Services.AddScoped<GenreService>();
            builder.Services.AddScoped<MediaGenresService>();
            builder.Services.AddScoped<SeriesDetailService>();
            builder.Services.AddScoped<SeasonService>();
            builder.Services.AddScoped<EpisodesService>();
            builder.Services.AddScoped<FilesService>();
            builder.Services.AddScoped<ImagesService>();
            builder.Services.AddScoped<PlaylistService>();
            builder.Services.AddScoped<PlaylistItemsService>();
            builder.Services.AddScoped<WatchProgressService>();
            builder.Services.AddScoped<HistoryService>();
            builder.Services.AddScoped<FoldersService>();

            // 配置 JWT 认证
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer              = "yourapp.com",
                        ValidAudience            = "yourapp.com",
                        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"))
                    };
                });

            // 配置 Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Media API", Version = "v1" });
            });

            // 注册控制器服务
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
        }

        /// <summary>
        /// 在主程序中构建完成后调用此方法以配置中间件管道
        /// </summary>
        public static void UseDatabasePipeline(WebApplication app)
        {
            // 启用 Swagger 中间件
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Media API V1");
            });

            // 启用认证和授权中间件
            app.UseAuthentication();
            app.UseAuthorization();

            // 强制 HTTPS 重定向
            app.UseHttpsRedirection();

            // 映射控制器路由
            app.MapControllers();
        }
    }
}