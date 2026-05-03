using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MinFramesFromShotChangeRule : IQcRule
{
    private readonly IShotChangeProvider _shotChangeProvider;
    private readonly int _thresholdFrames;

    public MinFramesFromShotChangeRule(IShotChangeProvider shotChangeProvider, int thresholdFrames)
    {
        _shotChangeProvider = shotChangeProvider;
        _thresholdFrames = thresholdFrames;
    }

    public string Name => "MinFramesFromShotChange";

    public IReadOnlyDictionary<string, RuleViolation> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<int> cuts = _shotChangeProvider.GetShotChangeFrames();
        return cues
            .Where(cue => ViolatesFrameDistance(cue, cuts))
            .ToDictionary(cue => cue.Id, _ => new RuleViolation(Name, "Cue starts too close to a shot cut."));
    }

    private bool ViolatesFrameDistance(Cue cue, IReadOnlyList<int> cuts)
    {
        if (!cue.StartFrame.HasValue || cuts.Count == 0)
        {
            return false;
        }

        int minDistance = cuts.Min(cut => Math.Abs(cue.StartFrame.Value - cut));
        return minDistance < _thresholdFrames;
    }
}
