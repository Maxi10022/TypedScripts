using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Common.Parser;
using static TypedScripts.Scripts.Generation.ScriptDiagnostics;

namespace TypedScripts.Scripts.Parser;

// Parses interpreter from shebang if available or explicitly user comment-driven
public class InterpreterParser : ILineParser
{
    private static readonly Regex InterpreterPattern = new(
        // line start, optional indent, a comment leader, optional space, '@interpreter', then space(s)
        CommentSyntax.LeaderPrefix + @"@interpreter\s+" +

        // the user-given interpreter name
        @"(?<interpreter>\S+)" +

        // optional trailing whitespace, then end of line
        @"\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ILineParseResult Parse(TextLine line)
    {
        var match = InterpreterPattern.Match(line.ToString());
        if (!match.Success) return ParseResults.Skip();

        var value = match.Groups["interpreter"].Value;

        return Enum.TryParse<Interpreter>(value, true, out var interpreter)
            ? ParseResults.Success(interpreter)
            : ParseResults.Failure(InterpreterNotSupported(value));
    }
}
