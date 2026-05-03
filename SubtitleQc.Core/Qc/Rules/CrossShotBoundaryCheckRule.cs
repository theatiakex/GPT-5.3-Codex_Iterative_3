using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class CrossShotBoundaryCheckRule : IQcRule
{
    private readonly IShotChangeProvider _shotChangeProvider;

    public CrossShotBoundaryCheckRule(IShotChangeProvider shotChangeProvider)
    {
        _shotChangeProvider = shotChangeProvider;
    }

    public string Name => "CrossShotBoundaryCheck";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<TimeSpan> cuts = _shotChangeProvider.GetShotChangeTimestamps();
        return cues
            .Where(cue => SpansAnyCut(cue, cuts))
            .ToDictionary(cue => cue.Id, _ => new RuleViolation(Name, "Cue crosses a shot boundary."));
    }

    private static bool SpansAnyCut(Cue cue, IReadOnlyList<TimeSpan> cuts)
    {
        return cuts.Any(cut => cue.Start < cut && cut < cue.End);
    }
}
