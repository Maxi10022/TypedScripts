using System;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Arguments.Exceptions;
using TypedScripts.Common.Exceptions;
using TypedScripts.Common.Parser;
using static System.StringComparison;

namespace TypedScripts.Arguments.Parser;

public class ArgumentParser : ILineParser
{
    private static readonly Regex ParamPattern = new(
        // line start, comment leader, '@param', then 'paramName:paramType'
        CommentSyntax.LeaderPrefix + @"@param\s+" +
        @"(?<paramName>[^\s:]+)\s*:\s*(?<paramType>\w+)" +
        // everything after the type is parsed token-by-token below
        @"(?<rest>.*)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // A token is a run of non-space chars and/or quoted segments,
    // so default="hello world" stays a single token.
    private static readonly Regex TokenPattern = new(
        @"(?:[^\s""]|""[^""]*"")+",
        RegexOptions.Compiled);
    
    public ILineParseResult Parse(TextLine line)
    {
        var match = ParamPattern.Match(line.ToString());
        if (!match.Success) return ParseResults.Skip();
        var options = ExtractOptions(match);
        return TryCreateArgument(options);
    }

    private ScriptArgumentOptions ExtractOptions(Match match)
    {
        var options = new ScriptArgumentOptions
        {
            Identifier = match.Groups["paramName"].Value,
            Type = match.Groups["paramType"].Value
        };

        // Parse optional settings to tokens
        var optional = match.Groups["rest"].Value;
        var tokens = TokenPattern.Matches(optional);
        
        foreach (Match token in tokens)
        {
            // Handle optional/required flags
            if (token.ValueEquals("required")) options.Required = true;
            if (token.ValueEquals("optional")) options.Required = false;
            
            // Handle default value
            const string defaultPrefix = "default=";
            if (token.StartsWith(defaultPrefix)) options.DefaultValue = token.GetTokenValue(defaultPrefix);
            
            // Handle named argument setting
            const string argPrefix = "argName=";
            if (token.StartsWith(argPrefix)) options.ArgName = token.GetTokenValue(argPrefix);
        }

        return options;
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