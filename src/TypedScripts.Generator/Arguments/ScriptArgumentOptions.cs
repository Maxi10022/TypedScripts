namespace TypedScripts.Arguments;

public class ScriptArgumentOptions
{
    /// <summary>
    /// The arguments C# type.
    /// </summary>
    public string? Type { get; set; }
    
    /// <summary>
    /// Argument identifier used in C#.
    /// </summary>
    public string? Identifier { get; set; }
    
    /// <summary>
    /// Require this argument or not.
    /// </summary>
    public bool Required { get; set; }
    
    /// <summary>
    /// Default value for this parameter.
    /// </summary>
    public string? DefaultValue { get; set; }
    
    /// <summary>
    /// Name of the CLI argument if defined as named argument.
    /// For named arguments only - replaces positional behavior.
    /// </summary>
    public string? ArgName { get; set; }
}