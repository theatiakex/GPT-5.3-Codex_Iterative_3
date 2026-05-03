using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc;

public sealed class RuleEngine
{
    private readonly IReadOnlyList<IQcRule> _rules;

    public RuleEngine(IReadOnlyList<IQcRule> rules)
    {
        _rules = rules;
    }

    public QcReport Evaluate(IReadOnlyList<Cue> cues)
    {
        var violations = CreateViolationBuckets(cues);
        ApplyRules(cues, violations);
        return BuildReport(cues, violations);
    }

    private static Dictionary<string, List<RuleViolation>> CreateViolationBuckets(IReadOnlyList<Cue> cues)
    {
        return cues.ToDictionary(c => c.Id, _ => new List<RuleViolation>());
    }

    private void ApplyRules(IReadOnlyList<Cue> cues, Dictionary<string, List<RuleViolation>> violations)
    {
        foreach (IQcRule rule in _rules)
        {
            AppendRuleViolations(rule.Evaluate(cues), violations);
        }
    }

    private static void AppendRuleViolations(
        IReadOnlyDictionary<string, RuleViolation> ruleOutput,
        Dictionary<string, List<RuleViolation>> violations)
    {
        foreach ((string cueId, RuleViolation violation) in ruleOutput)
        {
            if (violations.TryGetValue(cueId, out List<RuleViolation>? bucket))
            {
                bucket.Add(violation);
            }
        }
    }

    private static QcReport BuildReport(
        IReadOnlyList<Cue> cues,
        IReadOnlyDictionary<string, List<RuleViolation>> violations)
    {
        var results = new List<QcResult>(cues.Count);
        foreach (Cue cue in cues)
        {
            results.Add(new QcResult(cue.Id, violations[cue.Id]));
        }

        return new QcReport(results);
    }
}
