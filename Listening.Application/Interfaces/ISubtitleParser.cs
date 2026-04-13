namespace Listening.Application.Interfaces;
using Listening.Application.Dtos;
public interface ISubtitleParser
{
    Task<List<SubtitleParseResult>> ParseAsync(Stream stream, string fileName);
}