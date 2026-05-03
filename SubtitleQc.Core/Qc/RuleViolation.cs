namespace SubtitleQc.Core.Qc;

public sealed class RuleViolation
{
    public RuleViolation(string ruleName, string message)
    {
        RuleName = ruleName;
        Message = message;
    }

    public string RuleName { get; }

    public string Message { get; }
}
