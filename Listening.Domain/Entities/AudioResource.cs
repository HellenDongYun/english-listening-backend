namespace Listening.Domain.Entities;

public class AudioResource
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public string ContentType{ get; private set; }
    public long Size { get; private set; }
    private AudioResource() { }

    public AudioResource(string fileName, string contentType, long size)
    {
        Id= Guid.NewGuid();
        FileName = fileName;
        ContentType = contentType;
        Size = size;
    }
    public string GetUrl(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) return FileName;

        // 清理 baseUrl 结尾的斜杠
        var cleanedBase = baseUrl.TrimEnd('/');
        
        // 清理 FileName 开头的斜杠，并移除多余的 "upload/" 目录（如果你确定物理路径只有 uploads）
        var cleanedFile = FileName.TrimStart('/');
        
        // 针对你提到的错误：如果文件名里带了多余的 "upload/"，在这里剔除
        if (cleanedFile.StartsWith("upload/"))
        {
            cleanedFile = cleanedFile.Substring(7); // 移除 "upload/" 这 7 个字符
        }

        return $"{cleanedBase}/{cleanedFile}";
    }
}