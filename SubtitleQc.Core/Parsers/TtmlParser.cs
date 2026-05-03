using System.Globalization;
using System.Text;
using System.Xml.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

public sealed class TtmlParser : ISubtitleParser
{
    public string Format => "TTML";

    public IReadOnlyList<Cue> Parse(string content)
    {
        XDocument document = XDocument.Parse(content);
        XElement[] cueElements = FindCueElements(document).ToArray();
        var cues = new List<Cue>(cueElements.Length);

        for (int i = 0; i < cueElements.Length; i++)
        {
            Cue? cue = TryParseCue(cueElements[i], i);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }

        return cues;
    }

    private static IEnumerable<XElement> FindCueElements(XContainer root)
    {
        return root.Descendants().Where(e => e.Name.LocalName == "p");
    }

    private static Cue? TryParseCue(XElement element, int index)
    {
        if (!TryResolveCueTimes(element, out TimeSpan start, out TimeSpan end))
        {
            return null;
        }

        string id = ResolveCueId(element, index);
        IReadOnlyList<string> lines = ResolveCueLines(element);
        return new Cue(id, start, end, lines);
    }

    private static bool TryResolveCueTimes(XElement element, out TimeSpan start, out TimeSpan end)
    {
        string? beginValue = ReadAttribute(element, "begin");
        if (!TryParseTimeExpression(beginValue, out start))
        {
            end = default;
            return false;
        }

        return TryResolveCueEnd(element, start, out end);
    }

    private static bool TryResolveCueEnd(XElement element, TimeSpan start, out TimeSpan end)
    {
        string? endValue = ReadAttribute(element, "end");
        if (TryParseTimeExpression(endValue, out end))
        {
            return true;
        }

        string? durationValue = ReadAttribute(element, "dur");
        if (TryParseTimeExpression(durationValue, out TimeSpan duration))
        {
            end = start + duration;
            return true;
        }

        return false;
    }

    private static string ResolveCueId(XElement element, int index)
    {
        string? value = ReadAttribute(element, "id");
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return $"ttml-{index + 1}";
    }

    private static IReadOnlyList<string> ResolveCueLines(XElement element)
    {
        string normalized = ExtractText(element).Replace("\r\n", "\n");
        string[] lines = normalized.Split('\n', StringSplitOptions.None);
        return lines.Length == 0 ? new[] { string.Empty } : lines;
    }

    private static string ExtractText(XElement element)
    {
        var buffer = new StringBuilder();
        foreach (XNode node in element.Nodes())
        {
            AppendNodeText(node, buffer);
        }

        return buffer.ToString();
    }

    private static void AppendNodeText(XNode node, StringBuilder buffer)
    {
        if (node is XText text)
        {
            buffer.Append(text.Value);
            return;
        }

        if (node is not XElement element)
        {
            return;
        }

        if (element.Name.LocalName == "br")
        {
            buffer.Append('\n');
            return;
        }

        foreach (XNode child in element.Nodes())
        {
            AppendNodeText(child, buffer);
        }
    }

    private static string? ReadAttribute(XElement element, string localName)
    {
        XAttribute? exact = element.Attribute(localName);
        if (exact is not null)
        {
            return exact.Value;
        }

        return element.Attributes().FirstOrDefault(a => a.Name.LocalName == localName)?.Value;
    }

    private static bool TryParseTimeExpression(string? raw, out TimeSpan value)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            value = default;
            return false;
        }

        string input = raw.Trim();
        if (TimeSpan.TryParse(input, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        return TryParseOffsetTime(input, out value);
    }

    private static bool TryParseOffsetTime(string input, out TimeSpan value)
    {
        if (TryParseByUnit(input, "ms", 0.001, out value))
        {
            return true;
        }

        if (TryParseByUnit(input, "s", 1, out value))
        {
            return true;
        }

        if (TryParseByUnit(input, "m", 60, out value))
        {
            return true;
        }

        return TryParseByUnit(input, "h", 3600, out value);
    }

    private static bool TryParseByUnit(string input, string unit, double scale, out TimeSpan value)
    {
        if (!input.EndsWith(unit, StringComparison.OrdinalIgnoreCase))
        {
            value = default;
            return false;
        }

        string numeric = input[..^unit.Length];
        bool ok = double.TryParse(numeric, CultureInfo.InvariantCulture, out double amount);
        value = ok ? TimeSpan.FromSeconds(amount * scale) : default;
        return ok;
    }
}
