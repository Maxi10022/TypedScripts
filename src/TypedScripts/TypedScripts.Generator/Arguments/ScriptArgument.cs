using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypedScripts.Arguments.Exceptions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedScripts.Arguments;

public class ScriptArgument
{
    /// <summary>
    /// Position of this argument
    /// </summary>
    public int Position { get; } 
    
    /// <summary>
    /// Name of the CLI argument if defined as named argument.
    /// </summary>
    public string? ArgName { get; }
    
    /// <summary>
    /// Argument name used in C#.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The scripts parameter syntax.
    /// </summary>
    public ParameterSyntax Syntax { get; }
    
    /// <summary>
    /// Number of the line the script-arg was defined at.
    /// </summary>
    public int LineNumber { get; }

    /// <summary>
    /// Initializes and validates syntax for a defined script argument.
    /// </summary>
    /// <returns>Complete <see cref="ParameterSyntax"/></returns>
    /// <exception cref="UnsupportedArgumentTypeException">
    /// Thrown if the C# argument-type is not supported.
    /// </exception>
    /// <exception cref="UnsupportedArgumentDefaultException">
    /// Thrown if the arguments default value is not supported.
    /// </exception>
    /// <exception cref="InvalidArgumentDefaultException">
    /// Thrown when the default argument is invalid for the given C# type.
    /// (e.g. the default string "hobbit" for an integer)  
    /// </exception>
    public ScriptArgument(
        int position, 
        string type, 
        string name, 
        int lineNumber,
        bool required, 
        string? defaultValue, 
        string? argName)
    {
        Position = position;
        ArgName = argName;
        Name = name;
        LineNumber = lineNumber;
        Syntax = Build(type: type, name: name, required: required, defaultValue: defaultValue);
    }

    private static ParameterSyntax Build(string type, string name, bool required, string? defaultValue)
    {
        if (!SupportedArgumentTypes.IsSupportedArgumentType(type))
        {
            throw new UnsupportedArgumentTypeException(type);
        }

        if (!SyntaxFacts.IsValidIdentifier(name))
        {
            throw new InvalidParameterIdentifierException(name);
        }
        
        var syntaxType = GetTypeSyntax(type: type, required: required);
        
        var parameter = Parameter(Identifier(name))
            .WithType(syntaxType);

        return (defaultValue is not null 
            ? parameter.WithDefault(
                EqualsValueClause(GetDefaultLiteralExpression(type: type, defaultValue: defaultValue))) 
            : parameter).NormalizeWhitespace(); // Normalize white-spaces for both
    }
    
    public override string ToString() => Syntax.ToFullString();
    
    private static TypeSyntax GetTypeSyntax(string type, bool required) =>
        required 
            ? ParseTypeName(type) 
            : NullableType(ParseTypeName(type));

    private static LiteralExpressionSyntax GetDefaultLiteralExpression(string type, string defaultValue)
    {
        // Handle default value set to null.
        if (string.IsNullOrWhiteSpace(defaultValue) || defaultValue!.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression);
        }
        
        // defaultValue is not null since we call this method only in a protected branch!
        try
        {
            return GetFormattedType(type) switch
            {
                "string" => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(defaultValue!)),
                "char" => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal(char.Parse(defaultValue!))),
                "bool" => defaultValue!.Equals("true", StringComparison.OrdinalIgnoreCase) 
                    ? LiteralExpression(SyntaxKind.TrueLiteralExpression) 
                    : LiteralExpression(SyntaxKind.FalseLiteralExpression),
                "sbyte" => Num(Literal(sbyte.Parse(defaultValue!))),
                "short" => Num(Literal(short.Parse(defaultValue!))),
                "ushort" => Num(Literal(ushort.Parse(defaultValue!))),
                "int" => Num(Literal(int.Parse(defaultValue!))),
                "uint" => Num(Literal(uint.Parse(defaultValue!))),
                "long" => Num(Literal(long.Parse(defaultValue!))),
                "ulong" => Num(Literal(ulong.Parse(defaultValue!))),
                "float" => Num(Literal(float.Parse(defaultValue!))),
                "double" => Num(Literal(double.Parse(defaultValue!))),
                "decimal" => Num(Literal(decimal.Parse(defaultValue!))),
                
                // Should never be hit, just here for completeness
                _ => throw new UnsupportedArgumentDefaultException(type)
            };
        }
        catch 
        {
            throw new InvalidArgumentDefaultException(defaultValue, type);
        }

        static LiteralExpressionSyntax Num(SyntaxToken token) =>
            LiteralExpression(SyntaxKind.NumericLiteralExpression, token);
    }

    private static string GetFormattedType(string type) => type.ToLower();
}