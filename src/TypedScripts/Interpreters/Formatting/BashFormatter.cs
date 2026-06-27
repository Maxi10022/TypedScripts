namespace TypedScripts.Interpreters.Formatting;

internal class BashFormatter : IInterpreterFormatter
{
    public string Escape(string value)
    {
        if (value.Length == 0) return "''";
        return IsLiteralSafe(value) ? value : SingleQuote(value);
    }

    private static bool IsLiteralSafe(string value)
    {
        foreach (var c in value)
        {
            if (!IsLiteralSafe(c)) return false;
        }

        return true;
    }
    
    private static bool IsLiteralSafe(char c) =>
        c is >= 'A' and <= 'Z'
          or >= 'a' and <= 'z'
          or >= '0' and <= '9'
          or '_' or '@' or '%' or '+' or '=' or ':' or ',' or '.' or '/' or '-';

    private static string SingleQuote(string value) => "'" + value.Replace("'", "'\\''") + "'";
}
