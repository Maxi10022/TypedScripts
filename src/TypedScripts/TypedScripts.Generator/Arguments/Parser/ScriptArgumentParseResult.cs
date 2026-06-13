using Microsoft.CodeAnalysis;

namespace TypedScripts.Arguments.Parser;

public class ScriptArgumentParseResult
{
    /// <summary>
    /// Indicates parsing failed.
    /// </summary>
    public bool IsFailure => Argument is null;
    
    /// <summary>
    /// Indicates the argument was parsed.
    /// </summary>
    public bool IsSuccess => !IsFailure;
    
    /// <summary>
    /// Indicates there are problems but parsing still succeeded.
    /// </summary>
    public bool HasProblems => Problems.Length > 0;

    public DiagnosticDescriptor[] Problems { get; }
    
    public ScriptArgument? Argument { get; }
    
    private ScriptArgumentParseResult(ScriptArgument? argument, DiagnosticDescriptor[] problems)
    {
        Problems = problems;
        Argument = argument;
    }
    
    public static ScriptArgumentParseResult Success(
        ScriptArgument argument, params DiagnosticDescriptor[] problems) => new(argument, problems);
    
    public static ScriptArgumentParseResult Failure(params DiagnosticDescriptor[] problems) => new(null, problems);
}