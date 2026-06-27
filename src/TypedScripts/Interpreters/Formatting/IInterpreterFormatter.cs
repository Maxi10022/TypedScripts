namespace TypedScripts.Interpreters.Formatting;

internal interface IInterpreterFormatter
{
    /// <summary>
    /// Escapes <paramref name="value"/> so the interpreter receives it as a single, literal argument.
    /// </summary>
    string Escape(string value);
}
