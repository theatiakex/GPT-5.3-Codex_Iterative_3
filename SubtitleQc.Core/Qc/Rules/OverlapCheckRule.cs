using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class OverlapCheckRule : IQcRule
{
    public string Name => "OverlapCheck";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        var result = new Dictionary<string, RuleViolation>();
        Cue? previous = null;

        foreach (Cue cue in cues.OrderBy(c => c.Start))
        {
            if (previous is not null && cue.Start < previous.End)
            {
                result[cue.Id] = new RuleViolation(Name, "Cue overlaps with a previous cue.");
            }

            previous = cue;
        }

        return result;
    }
}
