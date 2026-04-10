using Listening.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
namespace Listening.Infrastructure.Storage;

public class LocalFileStorage:IFileStorage
{
    private readonly string _basePath;
    private readonly string _publicBaseUrl ="/uploads";
    // <summary>
    // 构造函数注入本地文件存储的根目录。
    // 在实际应用中，这个路径应该从配置中获取。
    // </summary>
    //<param name="environment">用于获取应用内容根目录</param>
    public LocalFileStorage(IHostEnvironment environment)
    {
        // 假设文件存储在应用程序的 ContentRootPath 下的 uploads 目录
        _basePath = Path.Combine(environment.ContentRootPath, "uploads");
        
        // 确保存储目录存在
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
       
    }
    public async Task<string> SaveFileAsync(Stream stream, string filename, string contentType)
    {
        // 1. 生成唯一且安全的文件名，并保留原始扩展名
        string extension = Path.GetExtension(filename);
        string safeFilename = $"{Guid.NewGuid():N}{extension}";
        string filePath = Path.Combine(_basePath, safeFilename);

        // 2. 将文件流写入本地文件
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            // 重置流的位置，确保从头开始复制
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            await stream.CopyToAsync(fileStream);
        }

        return safeFilename;
   
    }

    public Task DeleteFileAsync(string filename)
    {
        string filePath = Path.Combine(_basePath, filename);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // 本地文件删除操作是同步的，但为了满足接口的异步要求，返回 Task.CompletedTask
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string filename)
    {
        return $"{_publicBaseUrl}/{filename}";
    }
}