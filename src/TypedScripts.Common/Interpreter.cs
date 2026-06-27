// ReSharper disable once CheckNamespace

namespace TypedScripts;

/// <summary>
/// List of supported interpreters - extensible in the future.
/// </summary>
public enum Interpreter
{
    Bash
}

public static class InterpreterExtensions
{
    public static string GetName(this Interpreter interpreter) => interpreter switch
    {
        Interpreter.Bash => "Bash",
        _ => "Undefined"
    };

    public static string GetScriptExtension(this Interpreter interpreter) => interpreter switch
    {
        Interpreter.Bash => ".sh",
        _ => ".undefined",
    };
}