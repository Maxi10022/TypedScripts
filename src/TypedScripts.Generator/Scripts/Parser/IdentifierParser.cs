using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Common;
using TypedScripts.Common.Exceptions;
using TypedScripts.Common.Parser;
using static TypedScripts.Scripts.Generation.ScriptDiagnostics;

namespace TypedScripts.Scripts.Parser;

public class IdentifierParser : ILineParser
{
    private static readonly Regex IdentifierPattern = new(
        // line start, optional indent, a comment leader, optional space, '@identifier', then space(s)
        CommentSyntax.LeaderPrefix + @"@identifier\s+" +

        // the user-given identifier (captured loosely so invalid identifiers are reported downstream, not skipped)
        @"(?<identifier>\S+)" +

        // optional trailing whitespace, then end of line
        @"\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ILineParseResult Parse(TextLine line)
    {
        var match = IdentifierPattern.Match(line.ToString());
        if (!match.Success) return ParseResults.Skip();
        var value = match.Groups["identifier"].Value;
        
        try
        {
            var identifier = new SafeIdentifier(value);
            return ParseResults.Success(identifier);
        }
        catch (InvalidIdentifierException ex)
        {
            return ParseResults.Failure(InvalidScriptIdentifier(ex));
        }
    }
}
