using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxLinesRule : IQcRule
{
    private readonly int _threshold;

    public MaxLinesRule(int threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MaxLines";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        return cues
            .Where(c => c.Lines.Count > _threshold)
            .ToDictionary(c => c.Id, _ => new RuleViolation(Name, "Cue exceeds maximum lines."));
    }
}
