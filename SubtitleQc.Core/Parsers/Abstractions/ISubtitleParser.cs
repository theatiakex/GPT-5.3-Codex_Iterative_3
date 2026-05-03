using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers.Abstractions;

public interface ISubtitleParser
{
    string Format { get; }

    IReadOnlyList<Cue> Parse(string content);
}
