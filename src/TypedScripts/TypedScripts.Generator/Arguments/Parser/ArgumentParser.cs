using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Arguments.Exceptions;

namespace TypedScripts.Arguments.Parser;

public static class ArgumentParser
{
    private static readonly Regex ParamPattern = new(
        // line start, optional indent, '#', optional space, '@param', then space(s)
        @"^\s*#\s*@param\s+" + 
        
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
    
    // TODO Pass down file reference, which in turn gets passed down to diagnostics to make them helpful 
    public static IEnumerable<ScriptArgumentParseResult> ParseArguments(SourceText text)
    {
        var index = 0;
        
        foreach (var textLine in text.Lines)
        {
            var content = textLine.ToString();
            
            var match = ParamPattern.Match(content);
            if (!match.Success) continue;
            
            var result = ParseSingleArgument(match, index: index, lineNumber: textLine.LineNumber);
            if (result.IsSuccess) index++;
            
            yield return result;
        }
    }

    private static ScriptArgumentParseResult ParseSingleArgument(Match match, int index, int lineNumber)
    {
        ScriptArgument arg;

        try
        {
            // Extract argument options
            var name = match.Groups["paramName"].Value;
            var type = match.Groups["paramType"].Value;
            var required = match.Groups["modifier"].Value.Equals("required", StringComparison.OrdinalIgnoreCase);
            var defaultGroup = match.Groups["default"];
            var defaultValue = defaultGroup.Success ? defaultGroup.Value : null;
            var argNameGroup = match.Groups["argName"];
            var argName = argNameGroup.Success ? argNameGroup.Value : null;
            
            // Construct argument
            arg = new ScriptArgument(
                name: name,
                type: type,
                lineNumber: lineNumber,
                required: required,
                defaultValue: defaultValue,
                argName: argName,
                position: index
            );
        }
        // Handle argument construction issues
        catch (InvalidArgumentDefaultException ex)
        {
            return ScriptArgumentParseResult.Failure(
                lineNumber, ArgumentDiagnostics.InvalidArgumentDefault(ex));
        }
        catch (UnsupportedArgumentTypeException ex)
        {
            return ScriptArgumentParseResult.Failure(
                lineNumber, ArgumentDiagnostics.UnsupportedArgumentType(ex));
        }
        catch (InvalidParameterIdentifierException ex)
        {
            return ScriptArgumentParseResult.Failure(
                lineNumber, ArgumentDiagnostics.InvalidParameterIdentifier(ex));
        }
        catch (UnsupportedArgumentDefaultException ex)
        {
            return ScriptArgumentParseResult.Failure(
                lineNumber, ArgumentDiagnostics.UnsupportedArgumentDefault(ex));
        }
        catch (Exception ex)
        {
            // TODO add fallback diagnostic for unexpected exceptions, prompt user to open an issue. 
            throw new NotImplementedException();
        }
        
        // Return constructed argument
        return ScriptArgumentParseResult.Success(arg);
    }
}