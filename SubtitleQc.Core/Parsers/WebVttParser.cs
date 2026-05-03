using System.Globalization;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

public sealed class WebVttParser : ISubtitleParser
{
    public string Format => "WebVTT";

    public IReadOnlyList<Cue> Parse(string content)
    {
        var cues = new List<Cue>();
        foreach (string block in SplitBlocks(RemoveHeader(content)))
        {
            Cue? cue = ParseBlock(block);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }

        return cues;
    }

    private static string RemoveHeader(string content)
    {
        return content.Replace("\r\n", "\n").Replace("WEBVTT\n\n", string.Empty);
    }

    private static string[] SplitBlocks(string content)
    {
        return content.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    }

    private static Cue? ParseBlock(string block)
    {
        string[] lines = block.Split('\n', StringSplitOptions.None);
        int timingIndex = FindTimingLineIndex(lines);
        if (timingIndex < 0 || lines.Length <= timingIndex + 1)
        {
            return null;
        }

        return CreateCue(lines, timingIndex);
    }

    private static int FindTimingLineIndex(IReadOnlyList<string> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains(" --> ", StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static Cue CreateCue(IReadOnlyList<string> lines, int timingIndex)
    {
        (TimeSpan start, TimeSpan end) = ParseTiming(lines[timingIndex]);
        string id = timingIndex == 1 ? lines[0].Trim() : Guid.NewGuid().ToString("N");
        IReadOnlyList<string> textLines = lines.Skip(timingIndex + 1).ToArray();
        return new Cue(id, start, end, textLines);
    }

    private static (TimeSpan start, TimeSpan end) ParseTiming(string timingLine)
    {
        string[] parts = timingLine.Split(" --> ", StringSplitOptions.TrimEntries);
        return (ParseTime(parts[0]), ParseTime(parts[1]));
    }

    private static TimeSpan ParseTime(string value)
    {
        return TimeSpan.ParseExact(value, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
    }
}
