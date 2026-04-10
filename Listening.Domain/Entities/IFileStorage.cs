namespace Listening.Domain.Entities;

public interface IFileStorage
{
    Task<string> SaveFileAsync(Stream stream, string filename, string contentType);
    Task DeleteFileAsync(string filename);
    string GetPublicUrl(string filename);
}