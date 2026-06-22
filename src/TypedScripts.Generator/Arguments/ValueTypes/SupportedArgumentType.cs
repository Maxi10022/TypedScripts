using System;
using System.Linq;
using TypedScripts.Arguments.Exceptions;
using StringComparer = System.StringComparer;

namespace TypedScripts.Arguments.ValueTypes;

/// <summary>
/// Represents a primitive type which is supported to be used as shell argument in TypedScripts.  
/// </summary>
/// <param name="value">The C# type representation as string.</param>
/// <exception cref="UnsupportedArgumentTypeException">Thrown if the type is unsupported.</exception>
public class SupportedArgumentType(string? value) : IEquatable<SupportedArgumentType>
{
    public string Value { get; } = !IsSupportedType(value)
        ? throw new UnsupportedArgumentTypeException(value ?? "null")
        : value!.ToLowerInvariant(); // Normalize types to lowercase.
    
    public static readonly string[] All = 
    [
        "string",
        "bool",
        "byte",
        "sbyte",
        "char",
        "decimal",
        "double",
        "float",
        "int",
        "uint",
        "long",
        "ulong",
        "short",
        "ushort"
    ];
    
    private static bool IsSupportedType(string? type) =>
        All.Contains(type, StringComparer.InvariantCultureIgnoreCase);

    public bool Equals(SupportedArgumentType? other)
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
        return Equals((SupportedArgumentType)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();
}