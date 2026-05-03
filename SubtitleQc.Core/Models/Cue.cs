namespace SubtitleQc.Core.Models;

public sealed class Cue
{
    public Cue(string id, TimeSpan start, TimeSpan end, IReadOnlyList<string> lines)
    {
        Id = id;
        Start = start;
        End = end;
        Lines = lines;
    }

    public string Id { get; }

    public TimeSpan Start { get; }

    public TimeSpan End { get; }

    public IReadOnlyList<string> Lines { get; }

    public TimeSpan Duration => End - Start;
}
