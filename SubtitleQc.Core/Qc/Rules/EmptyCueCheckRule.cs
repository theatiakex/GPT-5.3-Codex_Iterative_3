using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class EmptyCueCheckRule : IQcRule
{
    public string Name => "EmptyCueCheck";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        return cues
            .Where(IsEmpty)
            .ToDictionary(c => c.Id, _ => new RuleViolation(Name, "Cue text is empty."));
    }

    private static bool IsEmpty(Cue cue)
    {
        return cue.Lines.All(string.IsNullOrWhiteSpace);
    }
}
