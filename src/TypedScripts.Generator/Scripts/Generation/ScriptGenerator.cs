using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TypedScripts.Arguments;
using TypedScripts.Arguments.Parser;
using TypedScripts.Common;
using TypedScripts.Common.Parser;
using TypedScripts.Scripts.Detection;
using TypedScripts.Scripts.Exceptions;
using TypedScripts.Scripts.Parser;

namespace TypedScripts.Scripts.Generation;

public class ScriptGenerator
{
    public IReadOnlyList<Script> Scripts => _scripts.AsReadOnly();

    private readonly List<Script> _scripts = new();
    private static readonly ArgumentParser ArgumentParser = new();
    private static readonly IdentifierParser IdentifierParser = new();
    private static readonly InterpreterParser InterpreterParser = new();
    private static readonly ShebangParser ShebangParser = new();
    
    public void Generate(SourceProductionContext ctx, ScriptCandidate candidate)
    {
        // Exit early if script content could not be read, report issue via diagnostics.
        if (candidate.SourceText is null)
        {
            var diagnostic = Diagnostic.Create(
                descriptor: ScriptDiagnostics.FailedToReadScriptContent(candidate.Path),
                location: GetDiagnosticFileLocation(candidate.Path));
            
            ctx.ReportDiagnostic(diagnostic);
            
            return;
        }

        var options = new ScriptOptions
        {
            FilePath = candidate.Path
        };
        
        // Configure all script options within a single loop
        foreach (var textLine in candidate.SourceText.Lines)
        {
            var content = textLine.ToString();
            
            // Append script body line
            options.Body.AppendLine(content);
            
            // Parse argument from script
            ParseLine<ScriptArgument>(
                parser: ArgumentParser,
                textLine: textLine,
                onSuccess: success => options.Arguments.Add(success.Value)
            );
            
            // Parse identifier from script
            ParseLine<SafeIdentifier>(
                parser: IdentifierParser,
                textLine: textLine,
                onSuccess: success => options.Identifier = success.Value
            );
            
            // Try parse interpreter from shebang 
            ParseLine<Interpreter>(
                parser: ShebangParser,
                textLine: textLine,
                onSuccess: success => options.Interpreters.Add(success.Value)
            );
            
            // Try parse interpreter from TypedScripts syntax 
            ParseLine<Interpreter>(
                parser: InterpreterParser,
                textLine: textLine,
                onSuccess: success => options.Interpreters.Add(success.Value)
            );
        }

        // Do the actual source-generation and error handling
        try
        {
            var script = Script.Create(options);
            _scripts.Add(script);
            ctx.AddSource(script.FileName, script.ToString());
        }
        catch (NoInterpreterAvailableException ex)
        {
            var diagnostic = ScriptDiagnostics.NoInterpreterAvailable(ex);
            ReportDiagnostic(ctx, candidate.Path, diagnostic);
        }
        catch (InvalidScriptSyntaxException ex)
        {
            var diagnostic = ScriptDiagnostics.InvalidScriptSyntax(ex);
            ReportDiagnostic(ctx, candidate.Path, diagnostic);
        }
        catch (Exception ex)
        {
            var message = BuildUnknownErrorMessage(ex);
            var diagnostic = ScriptDiagnostics.UnknownScriptValidationError(message);
            ReportDiagnostic(ctx, candidate.Path, diagnostic);
        }
        
        void ParseLine<T>(ILineParser parser, TextLine textLine, Action<LineParseSuccess<T>> onSuccess)
        {
            var result = parser.Parse(textLine);
            if (result is SkipLineParseResult) return;
            
            switch (result)
            {
                case LineParseFailure failure:
                    ReportParseFailure(ctx, candidate.Path, failure, textLine.LineNumber);
                    break;
                
                case LineParseSuccess<T> success:
                    onSuccess(success);
                    break;
            }
        }
    }

    private void ReportParseFailure(
        SourceProductionContext ctx, string scriptPath, LineParseFailure failure, int lineNumber)
    {
        foreach (var diagnosticDescriptor in failure.Descriptors)
        {
            ReportDiagnostic(ctx, scriptPath, diagnosticDescriptor, lineNumber);
        }
    }

    private static void ReportDiagnostic(
        SourceProductionContext ctx, string scriptPath, DiagnosticDescriptor descriptor, int? lineNumber = null)
    {
        var  location = GetDiagnosticFileLocation(scriptPath, lineNumber);
        var diagnostic = Diagnostic.Create(descriptor, location);
        ctx.ReportDiagnostic(diagnostic);
    }

    private string BuildUnknownErrorMessage(Exception ex) =>
        $"An unexpected exception occured while constructing script."
        + $"Problem: {ex}";
    
    private static Location GetDiagnosticFileLocation(string filePath, int? line = null)
    {
        var textSpan = TextSpan.FromBounds(0, 0);
        
        var lineValue = line ?? 0;
        var lineSpan = new LinePositionSpan(
            new LinePosition(lineValue, 0),
            new LinePosition(lineValue, 0));
     
        return Location.Create(filePath: filePath, textSpan, lineSpan);
    }
}
