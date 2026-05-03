using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MinDurationRule : IQcRule
{
    private readonly TimeSpan _threshold;

    public MinDurationRule(TimeSpan threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MinDuration";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        return cues
            .Where(c => c.Duration < _threshold)
            .ToDictionary(c => c.Id, _ => new RuleViolation(Name, "Cue is below minimum duration."));
    }
}
