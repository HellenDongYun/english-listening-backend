namespace Listening.Application.Dtos;

public class AudioResourceDto
{
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public string Url { get; set; } = null!;
}