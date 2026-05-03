namespace SubtitleQc.Core.Qc;

public sealed class QcResult
{
    public QcResult(string cueId, IReadOnlyList<RuleViolation> violations)
    {
        CueId = cueId;
        Violations = violations;
        Status = violations.Count == 0 ? QcStatus.Passed : QcStatus.Failed;
    }

    public string CueId { get; }

    public QcStatus Status { get; }

    public IReadOnlyList<RuleViolation> Violations { get; }
}
