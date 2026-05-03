using System.Globalization;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

public sealed class SrtParser : ISubtitleParser
{
    public string Format => "SRT";

    public IReadOnlyList<Cue> Parse(string content)
    {
        var cues = new List<Cue>();
        string[] blocks = SplitBlocks(content);

        foreach (string block in blocks)
        {
            Cue? cue = ParseBlock(block);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }

        return cues;
    }

    private static string[] SplitBlocks(string content)
    {
        return content.Replace("\r\n", "\n").Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    }

    private static Cue? ParseBlock(string block)
    {
        string[] lines = block.Split('\n', StringSplitOptions.None);
        if (lines.Length < 3)
        {
            return null;
        }

        return CreateCue(lines);
    }

    private static Cue CreateCue(string[] lines)
    {
        (TimeSpan start, TimeSpan end) = ParseTiming(lines[1]);
        IReadOnlyList<string> textLines = lines.Skip(2).ToArray();
        return new Cue(lines[0].Trim(), start, end, textLines);
    }

    private static (TimeSpan start, TimeSpan end) ParseTiming(string timingLine)
    {
        string[] parts = timingLine.Split(" --> ", StringSplitOptions.TrimEntries);
        return (ParseTime(parts[0]), ParseTime(parts[1]));
    }

    private static TimeSpan ParseTime(string value)
    {
        return TimeSpan.ParseExact(value, @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
    }
}
