using Microsoft.CodeAnalysis;

namespace TypedScripts.Arguments.Parser;

public class ScriptArgumentParseResult
{
    /// <summary>
    /// Indicates parsing failed.
    /// </summary>
    public bool IsFailure => Argument is null;
    
    /// <summary>
    /// The arguments line number.
    /// </summary>
    public int LineNumber { get; }
    
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
    
    private ScriptArgumentParseResult(ScriptArgument? argument, int lineNumber, DiagnosticDescriptor[] problems)
    {
        Problems = problems;
        Argument = argument;
        LineNumber = lineNumber;
    }
    
    public static ScriptArgumentParseResult Success(
        ScriptArgument argument, params DiagnosticDescriptor[] problems) => 
        new(argument, argument.LineNumber, problems);
    
    public static ScriptArgumentParseResult Failure(
        int lineNumber, params DiagnosticDescriptor[] problems) => 
        new(null, lineNumber, problems);
}