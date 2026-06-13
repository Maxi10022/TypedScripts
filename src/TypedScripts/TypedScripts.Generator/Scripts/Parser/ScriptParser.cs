using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Arguments.Parser;
using TypedScripts.Scripts.Exceptions;

namespace TypedScripts.Scripts.Parser;

public static class ScriptParser
{
    public static ScriptParseResult Parse(AdditionalText additionalText)
    {
        var sourceText = additionalText.GetText();

        // Exit early if script content could not be read.
        if (sourceText is null)
        {
            var diagnostic = Diagnostic.Create(
                descriptor: ScriptDiagnostics.FailedToReadScriptContent(additionalText.Path),
                location: GetFileLocation(additionalText.Path));
            
            return ScriptParseResult.Failure(diagnostic);
        }
        
        // Detect and parse all arguments.
        var argumentResults = ArgumentParser
            .ParseArguments(sourceText)
            .ToArray();
        
        // Collect argument diagnostics scoped to the script file and its specific line.
        var argumentDiagnostics = argumentResults
            .Where(x => x.HasProblems)
            .SelectMany(result => GetArgumentDiagnostics(additionalText.Path, result))
            .ToArray();
        
        // Exit early if argument parsing was fatal
        // We could still continue to generate the script but what's the point of args then? 
        var canContinue = argumentResults.All(x => x.IsSuccess);

        if (!canContinue)
        {
            return ScriptParseResult.Failure(argumentDiagnostics);
        }
        
        var args = argumentResults
            .Select(x => x.Argument!)
            .ToArray();

        // Parse script settings/info
        var name = GetScriptName(path: additionalText.Path, text: sourceText);
        var scriptContent = sourceText.ToString();

        try
        {
            // Construct script, throws on parsing issues.  
            var script = new Script(
                name: name,
                scriptContent: scriptContent,
                shell: Shell.Bash,
                arguments: args
            );

            // Also pass down diagnostics which were not fatal for argument parsing.
            return ScriptParseResult.Success(script, argumentDiagnostics);
        }
        catch (InvalidScriptIdentifierException ex)
        {
            var diagnostic = Diagnostic.Create(
                descriptor: ScriptDiagnostics.InvalidScriptIdentifier(ex),
                location: GetFileLocation(additionalText.Path));

            return ScriptParseResult.Failure([diagnostic, ..argumentDiagnostics]);
        }
        catch (InvalidScriptSyntaxException ex)
        {
            var diagnostic = Diagnostic.Create(
                descriptor: ScriptDiagnostics.InvalidScriptSyntax(ex),
                location: GetFileLocation(additionalText.Path));

            return ScriptParseResult.Failure([diagnostic, ..argumentDiagnostics]);
        }
        catch (Exception ex)
        {
            // TODO add fallback diagnostic for unexpected exceptions, prompt user to open an issue. 
            throw new NotImplementedException();
        }
    }

    private static string GetScriptName(string path, SourceText text)
    {
        // TODO format name
        var fallbackName = Path.GetFileNameWithoutExtension(path);
        
        // TODO go through all text lines and look for a script-name definition to use
        return fallbackName;
    }

    private static IEnumerable<Diagnostic> GetArgumentDiagnostics(
        string path, ScriptArgumentParseResult result) => 
        result.Problems.Select(problem => CreateArgumentDiagnostic(path, problem, result.LineNumber));

    private static Diagnostic CreateArgumentDiagnostic(string filePath, DiagnosticDescriptor descriptor, int line) => 
        Diagnostic.Create(
            descriptor: descriptor,
            location: GetFileLocation(filePath, line));

    private static Location GetFileLocation(string filePath, int? line = null)
    {
        var textSpan = TextSpan.FromBounds(0, 0);
        
        var lineValue = line ?? 0;
        var lineSpan = new LinePositionSpan(
            new LinePosition(lineValue, 0),
            new LinePosition(lineValue, 0));
     
        return Location.Create(filePath: filePath, textSpan, lineSpan);
    }
}
