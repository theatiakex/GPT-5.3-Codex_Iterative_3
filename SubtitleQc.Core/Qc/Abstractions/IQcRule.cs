using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Qc.Abstractions;

public interface IQcRule
{
    string Name { get; }

    IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues);
}
