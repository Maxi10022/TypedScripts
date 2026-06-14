using System;
using Microsoft.CodeAnalysis.CSharp;
using TypedScripts.Common.Exceptions;

namespace TypedScripts.Common;

/// <summary>
/// Represents an identifier safe to use in source generated code.
/// </summary>
/// <param name="value">The identifiers string representation.</param>
/// <exception cref="InvalidIdentifierException">Thrown if the identifier is invalid.</exception>
public class SafeIdentifier(string? value) : IEquatable<SafeIdentifier>
{
    public string Value { get; } =
        !SyntaxFacts.IsValidIdentifier(value)
            ? throw new InvalidIdentifierException(value ?? "null")
            : EscapeIdentifier(value); // Escape identifier if it's a reserved keyword.

    public static implicit operator SafeIdentifier(string value) => new(value);
    
    // Correctly escapes reserved keyword names
    public override string ToString() => Value;

    private static string EscapeIdentifier(string identifier)
    {
        var isReservedKeyword = SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None;
        return isReservedKeyword ? "@" + identifier : identifier;
    }
    
    public bool Equals(SafeIdentifier? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SafeIdentifier)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();
}