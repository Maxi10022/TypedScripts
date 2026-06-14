using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypedScripts.Arguments.Exceptions;
using TypedScripts.Arguments.ValueTypes;
using TypedScripts.Common;
using TypedScripts.Common.Exceptions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TypedScripts.Arguments;

public class ScriptArgument
{
    /// <summary>
    /// The optional CLI named-argument.
    /// </summary>
    public string? ArgName { get; }
    
    /// <summary>
    /// The C# identifier for this argument.
    /// </summary>
    public SafeIdentifier Identifier { get; }
    
    /// <summary>
    /// The C# argument type.
    /// </summary>
    public SupportedArgumentType Type { get; }
    
    /// <summary>
    /// The actual parameter syntax. (e.g. <c>string? animal = "cat"</c>)
    /// </summary>
    public ParameterSyntax Syntax { get; }
    
    private ScriptArgument(
        SafeIdentifier identifier, SupportedArgumentType type, ParameterSyntax syntax, string? argName)
    {
        ArgName = argName;
        Identifier = identifier;
        Syntax = syntax;
        Type = type;
    }

    /// <summary>
    /// Factory method to create new <see cref="ScriptArgument"/> instances.
    /// </summary>
    /// <exception cref="InvalidIdentifierException">Thrown if the identifier is invalid.</exception>
    /// <exception cref="UnsupportedArgumentTypeException">Thrown if the arguments type is unsupported.</exception>
    /// <exception cref="UnsupportedArgumentDefaultException">Thrown if the default value is not supported.</exception>
    public static ScriptArgument Create(ScriptArgumentOptions options)
    {
        var identifier = new SafeIdentifier(options.Identifier);
        var type = new SupportedArgumentType(options.Type);
        var syntax = BuildSyntax(identifier, type, options.Required, options.DefaultValue);
        return new ScriptArgument(identifier, type, syntax, options.ArgName);
    }

    private static ParameterSyntax BuildSyntax(
        SafeIdentifier identifier, SupportedArgumentType type, bool required, string? defaultValue)
    {
        var syntaxType = GetTypeSyntax(type: type.Value, required: required);
        
        var parameter = Parameter(Identifier(identifier.Value))
            .WithType(syntaxType);

        if (defaultValue is null) return parameter.NormalizeWhitespace();

        var defaultValueExpression =
            EqualsValueClause(GetDefaultLiteralExpression(type: type.Value, defaultValue: defaultValue));
        
        return parameter
            .WithDefault(defaultValueExpression)
            .NormalizeWhitespace();
    }
    
    /// <summary>
    /// Returns the valid C# argument representation for this instance.
    /// </summary>
    public override string ToString() => Syntax.ToFullString();
    
    private static TypeSyntax GetTypeSyntax(string type, bool required) => required 
        ? ParseTypeName(type) 
        : NullableType(ParseTypeName(type));

    private static LiteralExpressionSyntax GetDefaultLiteralExpression(string type, string defaultValue)
    {
        // Handle default value set to "null".
        if (string.IsNullOrWhiteSpace(defaultValue) || defaultValue!.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression);
        }
        
        try
        {
            return type switch
            {
                "string" => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(defaultValue)),
                "char" => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal(char.Parse(defaultValue))),
                "sbyte" => Num(Literal(sbyte.Parse(defaultValue))),
                "short" => Num(Literal(short.Parse(defaultValue))),
                "ushort" => Num(Literal(ushort.Parse(defaultValue))),
                "int" => Num(Literal(int.Parse(defaultValue))),
                "uint" => Num(Literal(uint.Parse(defaultValue))),
                "long" => Num(Literal(long.Parse(defaultValue))),
                "ulong" => Num(Literal(ulong.Parse(defaultValue))),
                "float" => Num(Literal(float.Parse(defaultValue))),
                "double" => Num(Literal(double.Parse(defaultValue))),
                "decimal" => Num(Literal(decimal.Parse(defaultValue))),
                "bool" => defaultValue.Equals("true", StringComparison.OrdinalIgnoreCase) 
                    ? LiteralExpression(SyntaxKind.TrueLiteralExpression) 
                    : LiteralExpression(SyntaxKind.FalseLiteralExpression),
                
                // Should never be hit, but acts as a safety net
                _ => throw new UnsupportedArgumentDefaultException(type)
            };
        }
        catch (Exception ex) when (ex is not UnsupportedArgumentDefaultException)
        {
            throw new InvalidArgumentDefaultException(defaultValue, type);
        }

        static LiteralExpressionSyntax Num(SyntaxToken token) =>
            LiteralExpression(SyntaxKind.NumericLiteralExpression, token);
    }
}