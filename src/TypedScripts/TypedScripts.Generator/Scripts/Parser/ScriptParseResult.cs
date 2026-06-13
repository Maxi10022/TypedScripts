using Microsoft.CodeAnalysis;

namespace TypedScripts.Scripts.Parser;

public class ScriptParseResult
{
    /// <summary>
    /// Indicates parsing failed.
    /// </summary>
    public bool IsFailure => Script is null;
    
    /// <summary>
    /// Indicates the argument was parsed.
    /// </summary>
    public bool IsSuccess => !IsFailure;
    
    /// <summary>
    /// Indicates there are diagnostic problems but parsing still succeeded.
    /// </summary>
    public bool HasProblems => Diagnostics.Length > 0;

    public Diagnostic[] Diagnostics { get; }
    
    public Script? Script { get; }
    
    private ScriptParseResult(Script? script, Diagnostic[] diagnostics)
    {
        Diagnostics = diagnostics;
        Script = script;
    }
    
    public static ScriptParseResult Success(
        Script script, params Diagnostic[] problems) => new(script, problems);
    
    public static ScriptParseResult Failure(params Diagnostic[] problems) => new(null, problems);
}