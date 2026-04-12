namespace Listening.Domain.Entities;

public class AudioResource
{
    public Guid Id { get; private set; }

    // 实际保存相对路径或文件名
    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long Size { get; private set; }

    private AudioResource()
    {
        // EF Core
    }

    public AudioResource(string fileName, string contentType, long size)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty.", nameof(contentType));

        if (size <= 0)
            throw new ArgumentException("File size must be greater than zero.", nameof(size));

        Id = Guid.NewGuid();
        FileName = NormalizeFileName(fileName);
        ContentType = contentType.Trim();
        Size = size;
    }

    public void Update(string fileName, string contentType, long size)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty.", nameof(contentType));

        if (size <= 0)
            throw new ArgumentException("File size must be greater than zero.", nameof(size));

        FileName = NormalizeFileName(fileName);
        ContentType = contentType.Trim();
        Size = size;
    }

    public string GetUrl(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            return FileName;

        var cleanedBase = baseUrl.TrimEnd('/');
        var cleanedFile = FileName.TrimStart('/');

        return $"{cleanedBase}/{cleanedFile}";
    }

    private static string NormalizeFileName(string fileName)
    {
        var normalized = fileName.Trim().Replace("\\", "/");

        // 去掉开头多余的斜杠
        normalized = normalized.TrimStart('/');

        return normalized;
    }
}