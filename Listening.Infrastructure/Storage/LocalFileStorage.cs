using Listening.Domain.Entities;
using Microsoft.Extensions.Hosting;

namespace Listening.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _basePath;
    private readonly string _publicBaseUrl = "/uploads";

    public LocalFileStorage(IHostEnvironment environment)
    {
        _basePath = Path.Combine(environment.ContentRootPath, "uploads");

        Directory.CreateDirectory(_basePath);
        Directory.CreateDirectory(Path.Combine(_basePath, "audio"));
        Directory.CreateDirectory(Path.Combine(_basePath, "subtitles"));
        Directory.CreateDirectory(Path.Combine(_basePath, "images"));
        Directory.CreateDirectory(Path.Combine(_basePath, "other"));
    }

    public async Task<string> SaveFileAsync(Stream stream, string filename, string contentType)
    {
        string extension = Path.GetExtension(filename);
        string safeFilename = $"{Guid.NewGuid():N}{extension}";

        string folderName = GetFolderName(contentType, extension);

        string folderPath = Path.Combine(_basePath, folderName);
        string filePath = Path.Combine(folderPath, safeFilename);

        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);

        // 重点：这里返回带文件夹的路径，例如 audio/xxx.m4a
        return $"{folderName}/{safeFilename}";
    }

    public Task DeleteFileAsync(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return Task.CompletedTask;
        }

        string filePath = Path.Combine(_basePath, filename);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    public string GetPublicUrl(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            return string.Empty;
        }

        return $"{_publicBaseUrl}/{filename.Replace("\\", "/")}";
    }

    private static string GetFolderName(string contentType, string extension)
    {
        contentType = contentType?.ToLowerInvariant() ?? "";
        extension = extension?.ToLowerInvariant() ?? "";

        if (contentType.StartsWith("audio/") ||
            extension is ".mp3" or ".m4a" or ".wav" or ".aac" or ".ogg")
        {
            return "audio";
        }

        if (contentType.Contains("subtitle") ||
            extension is ".srt" or ".vtt")
        {
            return "subtitles";
        }

        if (contentType.StartsWith("image/") ||
            extension is ".jpg" or ".jpeg" or ".png" or ".webp")
        {
            return "images";
        }

        return "other";
    }
}