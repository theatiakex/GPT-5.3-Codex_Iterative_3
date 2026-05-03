namespace SubtitleQc.Core.Qc;

public sealed class QcReport
{
    public QcReport(IReadOnlyList<QcResult> results)
    {
        Results = results;
    }

    public IReadOnlyList<QcResult> Results { get; }
}
