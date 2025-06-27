using Media.Data;
using Media.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// �������ݿ�����
builder.Services.AddDbContext<MediaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ע��ҵ���߼�����
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<MediaItemService>();

// ���� JWT ��֤
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourapp.com",  // ����Ը���Ϊʵ�ʵ����������
            ValidAudience = "yourapp.com",  // ����Ը���Ϊʵ�ʵ����������
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"))
        };
    });

// ��� Swagger ����
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Media API", Version = "v1" });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ���� Swagger �м��
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Media API V1");
});

// ������֤�м��
app.UseAuthentication();  // ȷ����֤����Ȩ֮ǰ
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
