using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MediaNetServer.Data.media.Data;
using Microsoft.AspNetCore.Builder;
using MediaNetServer.Data.media.Services;

namespace MediaNetServer.Data.media;

/// <summary>
/// 把原来 Program.cs 里的“配置服务”和“配置中间件”逻辑拆出来，
/// 通过显式调用的方式在根目录 Program.cs 里执行它们。
/// </summary>
public static class DatabaseInit
{
    /// <summary>
    /// 注册数据库、业务服务、认证、Swagger、Controllers 等。
    /// </summary>
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置数据库连接
        services.AddDbContext<MediaContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 注册业务逻辑服务
        services.AddScoped<UserService>();
        services.AddScoped<TokenService>();
        services.AddScoped<MediaItemService>();

        // 配置 JWT 认证
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourapp.com",
                    ValidAudience = "yourapp.com",
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"))
                };
            });

        // 添加 Swagger 服务
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Media API", Version = "v1" });
        });

        // MVC 控制器支持
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    /// <summary>
    /// 注册中间件管道：Swagger、认证、路由等
    /// </summary>
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Media API V1");
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHttpsRedirection();
        app.MapControllers();
    }
}