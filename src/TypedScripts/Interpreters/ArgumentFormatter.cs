#nullable enable
using System;
using System.Globalization;
using TypedScripts.Interpreters.Formatting;

namespace TypedScripts.Interpreters;

public class ArgumentFormatter(Interpreter interpreter)
{
    private readonly IInterpreterFormatter _formatter = Resolve(interpreter);

    /// <summary>
    /// Formats <paramref name="value"/> to be shell safe.
    /// </summary>
    /// <param name="value">The typed argument value (e.g. string, int, double, bool, char).</param>
    /// <returns>A shell-safe token representing <paramref name="value"/>.</returns>
    public string Escape<T>(T value) => Escape(Format(value));

    /// <summary>
    /// Formats <paramref name="value"/> to be shell safe.
    /// </summary>
    /// <param name="value">The raw string value; <c>null</c> is treated as an empty argument.</param>
    /// <returns>A shell-safe token representing <paramref name="value"/>.</returns>
    public string Escape(string? value) => _formatter.Escape(value ?? string.Empty);

    private static IInterpreterFormatter Resolve(Interpreter interpreter) => interpreter switch
    {
        Interpreter.Bash => new BashFormatter(),
        _ => throw new NotSupportedException(
            $"Argument escaping for interpreter '{interpreter.GetName()}' is not supported.")
    };

    private static string Format<T>(T value) => value switch
    {
        null => string.Empty,
        string s => s,
        bool b => b ? "true" : "false",
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty
    };
}
