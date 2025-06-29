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
/// ��ԭ�� Program.cs ��ġ����÷��񡱺͡������м�����߼��������
/// ͨ����ʽ���õķ�ʽ�ڸ�Ŀ¼ Program.cs ��ִ�����ǡ�
/// </summary>
public static class DatabaseInit
{
    /// <summary>
    /// ע�����ݿ⡢ҵ�������֤��Swagger��Controllers �ȡ�
    /// </summary>
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // �������ݿ�����
        services.AddDbContext<MediaContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ע��ҵ���߼�����
        services.AddScoped<UserService>();
        services.AddScoped<TokenService>();
        services.AddScoped<MediaItemService>();

        // ���� JWT ��֤
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

        // ��� Swagger ����
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Media API", Version = "v1" });
        });

        // MVC ������֧��
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    /// <summary>
    /// ע���м���ܵ���Swagger����֤��·�ɵ�
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