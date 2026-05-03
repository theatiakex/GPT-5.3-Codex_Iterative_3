using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxCpsRule : IQcRule
{
    private readonly double _threshold;

    public MaxCpsRule(double threshold)
    {
        _threshold = threshold;
    }

    public string Name => "MaxCps";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        return cues
            .Where(ExceedsMaxCps)
            .ToDictionary(c => c.Id, _ => new RuleViolation(Name, "Cue exceeds maximum CPS."));
    }

    private bool ExceedsMaxCps(Cue cue)
    {
        double seconds = cue.Duration.TotalSeconds;
        if (seconds <= 0)
        {
            return true;
        }

        return CountCharacters(cue) / seconds > _threshold;
    }

    private static int CountCharacters(Cue cue)
    {
        return cue.Lines.Sum(line => line.Length);
    }
}
