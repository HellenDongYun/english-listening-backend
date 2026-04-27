using Listening.Domain.Entities;
using Listening.Infrastructure;
using Listening.Infrastructure.Repositories;
using Listening.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Listening.Application.Exercise;
using Listening.Application.Interfaces;
using Listening.Infrastructure.Subtitle;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// --- 1. 注册服务 (Services Configuration) ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 应用层服务
builder.Services.AddScoped<ExerciseService>();

// 数据库上下文配置
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        b => b.MigrationsAssembly("Listening.Infrastructure"))
);

// 依赖注入 (Dependency Injection)
builder.Services.AddScoped<ILessonRepository, EfLessonRepository>();
builder.Services.AddSingleton<LocalFileStorage>();
builder.Services.AddScoped<IFileStorage>(sp => sp.GetRequiredService<LocalFileStorage>());
builder.Services.AddScoped<ISubtitleParser, SrtSubtitleParser>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Listening.Application.Profiles.LessonProfile).Assembly);

// 跨域策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
    // 或者允许所有来源（开发环境）
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- 2. 配置中间件管道 (Middleware Pipeline) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 静态文件配置
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".m4a"] = "audio/mp4";
provider.Mappings[".srt"] = "text/plain";
provider.Mappings[".vtt"] = "text/vtt";

var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

// 默认静态文件 (针对 wwwroot)
app.UseStaticFiles();

// 自定义静态文件映射 (针对 /uploads 目录)
app.UseStaticFiles(new StaticFileOptions
{
    // 修改 1：明确使用 uploads 物理目录
    FileProvider = new PhysicalFileProvider(uploadPath),

    // 修改 2：浏览器访问路径仍然是 /uploads
    RequestPath = "/uploads",

    // 修改 3：把你上面配置的 .m4a 类型真正用起来
    ContentTypeProvider = provider,

    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, HEAD");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type");
    }
});

app.UseRouting();

// Cors 必须在 UseRouting 之后，UseAuthorization 之前
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();



