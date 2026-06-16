namespace TypedScripts.Common.Parser;

/// <summary>
/// Shared regex fragments for parsing comment-embedded <c>@directive</c> annotations
/// (e.g. <c>@param</c>, <c>@identifier</c>, <c>@interpreter</c>) across the comment
/// syntaxes of the supported interpreters.
/// </summary>
public static class CommentSyntax
{
    /// <summary>
    /// Matches the line start, optional indentation and a single-line comment leader for any
    /// supported interpreter, followed by optional space:
    /// <c>#</c> (shell/python/powershell), <c>//</c> (js/ts), <c>--</c> (sql/lua),
    /// <c>;</c> (lisp/ini), <c>%</c> (matlab/latex), <c>::</c>/<c>REM</c> (batch).
    /// </summary>
    /// <remarks>
    /// Patterns using this fragment must enable
    /// <see cref="System.Text.RegularExpressions.RegexOptions.IgnoreCase"/> so the
    /// <c>REM</c> batch leader matches case-insensitively.
    /// </remarks>
    public const string LeaderPrefix = @"^\s*(?:#|//|--|;|%|::|REM\b)\s*";
}
