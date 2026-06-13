using Microsoft.CodeAnalysis;
using TypedScripts.Arguments.Exceptions;

namespace TypedScripts.Arguments;

public static class ArgumentDiagnostics
{
    public static DiagnosticDescriptor UnsupportedArgumentType(UnsupportedArgumentTypeException ex) => new(
        id: "TSARG001",
        title: "Unsupported argument type",
        messageFormat: ex.Message,
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor InvalidArgumentDefault(InvalidArgumentDefaultException ex) => new(
        id: "TSARG002",
        title: "Invalid default value",
        messageFormat: ex.Message,
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor InvalidParameterIdentifier(InvalidParameterIdentifierException ex) => new(
        id: "TSARG003",
        title: "Invalid parameter identifier",
        messageFormat: ex.Message,
        category: "TypedScripts", 
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor UnsupportedArgumentDefault(UnsupportedArgumentDefaultException ex) => new(
        id: "TSARG004",
        title: "Unsupported argument default",
        messageFormat: ex.Message,
        category: "TypedScripts",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}