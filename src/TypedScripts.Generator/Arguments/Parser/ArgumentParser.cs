using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Arguments.Exceptions;
using TypedScripts.Common.Exceptions;
using TypedScripts.Common.Parser;

namespace TypedScripts.Arguments.Parser;

public class ArgumentParser : ILineParser
{
    private static readonly Regex ParamPattern = new(
        // line start, optional indent, a comment leader, optional space, '@param', then space(s)
        CommentSyntax.LeaderPrefix + @"@param\s+" +
        
        // paramName (captured loosely so invalid identifiers are reported, not skipped), ':', paramType
        @"(?<paramName>[^\s:]+)\s*:\s*(?<paramType>\w+)" +
        
        // optional: space(s) then the literal 'required' or 'optional'
        @"(?:\s+(?<modifier>required|optional))?" +
        
        // optional: ' default=' followed by a quoted value (spaces allowed) or a non-space value
        @"(?:\s+default=(?:""(?<default>[^""]*)""|(?<default>\S+)))?" +
        
        // optional: ' argName=' followed by a word (hyphens allowed)
        @"(?:\s+argName=(?<argName>[\w-]+))?" +
        
        // optional trailing whitespace, then end of line
        @"\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public ILineParseResult Parse(TextLine line)
    {
        var match = ParamPattern.Match(line.ToString());
        if (!match.Success) return ParseResults.Skip();
        var options = ExtractOptions(match);
        return TryCreateArgument(options);
    }

    private ScriptArgumentOptions ExtractOptions(Match match)
    {
        var defaultGroup = match.Groups["default"];
        var argNameGroup = match.Groups["argName"];
        
        return new ScriptArgumentOptions
        {
            Identifier = match.Groups["paramName"].Value,
            Type = match.Groups["paramType"].Value,
            Required = match.Groups["modifier"].Value.Equals("required", StringComparison.OrdinalIgnoreCase),
            DefaultValue = defaultGroup.Success ? defaultGroup.Value : null,
            ArgName = argNameGroup.Success ? argNameGroup.Value : null,
        };
        
    }

    private ILineParseResult TryCreateArgument(ScriptArgumentOptions options)
    {
        try
        {
            var script = ScriptArgument.Create(options);
            return ParseResults.Success(script);
        }
        // Handle argument construction issues
        catch (InvalidArgumentDefaultException ex)
        {
            return ParseResults.Failure(
                ArgumentDiagnostics.InvalidArgumentDefault(ex));
        }
        catch (UnsupportedArgumentTypeException ex)
        {
            return ParseResults.Failure(
                ArgumentDiagnostics.UnsupportedArgumentType(ex));
        }
        catch (InvalidIdentifierException ex)
        {
            return ParseResults.Failure(
                ArgumentDiagnostics.InvalidParameterIdentifier(ex));
        }
        catch (UnsupportedArgumentDefaultException ex)
        {
            return ParseResults.Failure(
                ArgumentDiagnostics.UnsupportedArgumentDefault(ex));
        }
        catch (Exception ex)
        {
            return ParseResults.Failure(
                ArgumentDiagnostics.UnknownArgumentValidationError(BuildUnknownErrorMessage(ex)));
        }
    }

    private string BuildUnknownErrorMessage(Exception ex) =>
        $"An unexpected exception occured while validating argument."
        + $"Problem: {ex}";
}