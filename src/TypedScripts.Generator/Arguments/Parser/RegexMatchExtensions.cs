using System.Text.RegularExpressions;
using static System.StringComparison;

namespace TypedScripts.Arguments.Parser;

public static class RegexMatchExtensions
{
    // Extensions
    public static string GetTokenValue(
        this Match match, string prefix) =>
        StripQuotes(match.Value.Substring(prefix.Length));
    
    public static bool ValueEquals(
        this Match match, string expected) => 
        match.Value.Equals(expected, OrdinalIgnoreCase);
    
    public static bool StartsWith(
        this Match match, string expected) => 
        match.Value.StartsWith(expected, OrdinalIgnoreCase);
    
    // Helpers
    private static string StripQuotes(string value) =>
        value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"'
            ? value.Substring(1, value.Length - 2)
            : value;
}