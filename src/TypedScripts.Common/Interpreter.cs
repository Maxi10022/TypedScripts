// ReSharper disable once CheckNamespace
namespace TypedScripts;

/// <summary>
/// List of supported interpreters - extensible in the future.
/// </summary>
public enum Interpreter
{
    Bash
}

public static class ShellExtensions
{
    public static string GetShellName(this Interpreter interpreter) => interpreter switch
    {
        Interpreter.Bash => "Bash",
        _ => "Undefined"
    };
}