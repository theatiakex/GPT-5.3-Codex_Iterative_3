using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxCplRule : IQcRule
{
    private readonly int _threshold;

    public MaxCplRule(int threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MaxCpl";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        return cues
            .Where(HasOverlongLine)
            .ToDictionary(c => c.Id, _ => new RuleViolation(Name, "A line exceeds maximum CPL."));
    }

    private bool HasOverlongLine(Cue cue)
    {
        return cue.Lines.Any(line => line.Length > _threshold);
    }
}
