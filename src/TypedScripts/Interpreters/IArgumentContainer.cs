namespace TypedScripts.Interpreters;

/// <summary>
/// Used for code-generated argument DTO objects.
/// </summary>
public interface IArgumentContainer
{
    /// <summary>
    /// Convert the DTO to an argument string for the script to execute.
    /// </summary>
    /// <param name="formatter">Argument formatter for the scripts' interpreter.</param>
    /// <returns>Single-line argument string passed to the script.</returns>
    public string ToArgumentString(ArgumentFormatter formatter);
}