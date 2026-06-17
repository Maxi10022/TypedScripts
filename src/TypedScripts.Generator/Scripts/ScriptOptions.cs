using System.Collections.Generic;
using System.Text;
using TypedScripts.Arguments;
using TypedScripts.Common;

namespace TypedScripts.Scripts;

public class ScriptOptions
{
    /// <summary>
    /// The script identifier.
    /// </summary>
    public SafeIdentifier? Identifier { get; set; }

    /// <summary>
    /// The scripts actual body.
    /// </summary>
    public StringBuilder Body { get; set; } = new();
    
    /// <summary>
    /// The path to the script.
    /// Required as a fallback if either <c>Identifier</c> or <c>Interpreters</c> were not defined.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Can be used to explicitly provide supported interpreters.
    /// </summary>
    public List<Interpreter> Interpreters { get; set; } = [];
    
    /// <summary>
    /// The scripts arguments detected by TypedScripts.
    /// </summary>
    public List<ScriptArgument> Arguments { get; set; } = [];
}