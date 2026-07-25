using TypedScripts.Interpreters;

namespace TypedScripts.Executors;

/// <summary>
/// Usually constructed by the code-generated script object. 
/// </summary>
/// <param name="script">The actual script content to execute.</param>
/// <param name="interpreter">The interpreter to use.</param>
/// <param name="arguments">An argument container required to support a method to build the arg string.</param>
public class ExecutionOptions(string script, Interpreter interpreter, IArgumentContainer arguments)
{
    /// <summary>
    /// The actual script to execute.
    /// </summary>
    public string Script { get; } = script;
    
    /// <summary>
    /// The shell to use to execute that script.
    /// </summary>
    public Interpreter Interpreter { get; } = interpreter;
    
    /// <summary>
    /// Ordered list of script arguments, order must be preserved.
    /// This means the first item must be the first argument etc.
    /// </summary>
    public IArgumentContainer Arguments { get; } = arguments;
}