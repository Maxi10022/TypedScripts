using Microsoft.CodeAnalysis;
using TypedScripts.Scripts.Exceptions;

namespace TypedScripts.Scripts;

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
    
    public static DiagnosticDescriptor InvalidScriptIdentifier(InvalidScriptIdentifierException ex) => new(
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
}