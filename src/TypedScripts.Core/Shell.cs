namespace TypedScripts.Core;

/// <summary>
/// List of supported shells - extensible in the future.
/// </summary>
public enum Shell
{
    Bash
}

public static class ShellExtensions
{
    public static string GetShellName(this Shell shell) => shell switch
    {
        Shell.Bash => "Bash",
        _ => "Undefined"
    };
}