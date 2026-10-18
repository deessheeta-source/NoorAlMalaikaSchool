using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NoorAlMalaikaSchool.Services
{
    /// <summary>
    /// Renders message bodies for WhatsApp. Supports named tokens ({guardianName}) and
    /// Meta positional tokens ({{1}}, {{2}}) that point at the named tokens in order of
    /// first appearance. Each named token contributes exactly one template parameter,
    /// even when it appears several times in the body.
    /// </summary>
    public static class WhatsAppTemplateRenderer
    {
        private static readonly Regex TokenRegex = new Regex(
            @"\{\{([0-9]{1,2}|[A-Za-z][A-Za-z0-9_]*)\}\}|\{([A-Za-z][A-Za-z0-9_]*)\}",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string Render(string body, IDictionary<string, string> values, out List<string> orderedParameters)
        {
            orderedParameters = new List<string>();
            if (string.IsNullOrEmpty(body)) return string.Empty;

            var collected = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match match in TokenRegex.Matches(body))
            {
                string name = GroupValue(match, 2);
                if (name.Length == 0) continue;
                if (collected.ContainsKey(name)) continue;
                string value;
                if (!values.TryGetValue(name, out value)) value = string.Empty;
                collected[name] = value ?? string.Empty;
                orderedParameters.Add(collected[name]);
            }

            var parameters = orderedParameters;
            return TokenRegex.Replace(body, delegate(Match match)
            {
                string inner = GroupValue(match, 1);
                if (inner.Length > 0)
                {
                    int index;
                    if (int.TryParse(inner, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                    {
                        return index >= 1 && index <= parameters.Count
                            ? parameters[index - 1] : string.Empty;
                    }
                    string collectedValue;
                    if (collected.TryGetValue(inner, out collectedValue)) return collectedValue;
                    return string.Empty;
                }
                string value;
                return values.TryGetValue(GroupValue(match, 2), out value) ? (value ?? string.Empty) : string.Empty;
            });
        }

        private static string GroupValue(Match match, int groupNumber)
        {
            Group group = match.Groups[groupNumber];
            return group.Success ? group.Value : string.Empty;
        }
    }
}
