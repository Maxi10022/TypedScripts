using Microsoft.CodeAnalysis;
using TypedScripts.Common.Exceptions;
using TypedScripts.Scripts.Exceptions;

namespace TypedScripts.Scripts.Generation;

public static class ScriptDiagnostics
{
    public static DiagnosticDescriptor FailedToReadScriptContent(string fileName) => new(
        id: "TSC001",
        title: "Failed to read script content",
        messageFormat: $"Content from '{fileName}' could not be read.",
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor InvalidScriptIdentifier(InvalidIdentifierException ex) => new(
        id: "TSC002",
        title: "Invalid script identifier",
        messageFormat: ex.Message,
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor InvalidScriptSyntax(InvalidScriptSyntaxException ex) => new(
        id: "TSC003",
        title: "Invalid script syntax",
        messageFormat: ex.Message,
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor NoInterpreterAvailable(NoInterpreterAvailableException ex) => new(
        id: "TSC004",
        title: "No interpreter available",
        messageFormat: ex.Message,
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor InterpreterNotSupported(string interpreter) => new(
        id: "TSC005",
        title: "Interpreter not supported",
        messageFormat: $"Specified interpreter '{interpreter}' is not supported.",
        category: "TypedScripts",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor UnknownScriptValidationError(string message) => new(
        id: "TSC006",
        title: "Unknown problem during script validation",
        messageFormat: message, 
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}