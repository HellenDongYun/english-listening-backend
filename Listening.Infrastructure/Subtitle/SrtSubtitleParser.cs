namespace Listening.Infrastructure.Subtitle;
using System;
using System.Text;
using Listening.Application.Interfaces;
using Listening.Application.Dtos;
public class SrtSubtitleParser: ISubtitleParser
{
    public async Task<List<SubtitleParseResult>> ParseAsync(Stream stream, string fileName)
    {
        var results = new List<SubtitleParseResult>();

        using var reader = new StreamReader(stream, Encoding.UTF8, true, leaveOpen: true);
        var content = await reader.ReadToEndAsync();

        var blocks = content.Split(
            new[] { "\r\n\r\n", "\n\n" },
            StringSplitOptions.RemoveEmptyEntries
        );

        foreach (var block in blocks)
        {
            var lines = block.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries
            );

            if (lines.Length < 3)
                continue;

            if (!int.TryParse(lines[0], out var sequence))
                continue;

            var timeParts = lines[1].Split(" --> ");
            if (timeParts.Length != 2)
                continue;

            var start = ParseSrtTime(timeParts[0]);
            var end = ParseSrtTime(timeParts[1]);
            var text = string.Join(" ", lines.Skip(2)).Trim();

            results.Add(new SubtitleParseResult
            {
                Sequence = sequence,
                StartTime = start,
                EndTime = end,
                Text = text
            });
        }

        return results;
    }

    private static TimeSpan ParseSrtTime(string value)
    {
        value = value.Trim().Replace(',', '.');
        return TimeSpan.Parse(value);
    }
    
}
