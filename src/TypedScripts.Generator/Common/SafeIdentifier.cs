using System;
using Microsoft.CodeAnalysis.CSharp;
using TypedScripts.Common.Exceptions;
using TypedScripts.Scripts.Exceptions;

namespace TypedScripts.Common;

public class SafeIdentifier(string? value) : IEquatable<SafeIdentifier>
{
    public string Value { get; } =
        !SyntaxFacts.IsValidIdentifier(value)
            ? throw new InvalidIdentifierException(value ?? "null")
            : value;

    // Reserved keywords must be escaped properly
    private readonly bool _isReservedKeyword = SyntaxFacts.GetKeywordKind(value) != SyntaxKind.None;

    public static implicit operator SafeIdentifier(string value) => new(value);
    
    // Correctly escapes reserved keyword names
    public override string ToString() => _isReservedKeyword ? "@" + Value : Value;
    
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