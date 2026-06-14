using System;
using System.Collections.Generic;
using TypedScripts.Arguments;
using TypedScripts.Arguments.Exceptions;
using TypedScripts.Arguments.ValueTypes;
using TypedScripts.Common.Exceptions;
using Xunit;

namespace TypedScripts.Tests.Arguments;

public class ScriptArgumentTests
{
    [Fact]
    public void Using_Unsupported_Type_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<UnsupportedArgumentTypeException>(() =>
            Argument(type: "hobbits", identifier: "species"));
    }

    [Fact]
    public void Using_Invalid_Default_Value_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidArgumentDefaultException>(() =>
            Argument(type: "int", identifier: "species", defaultValue: "gandalf"));
    }

    [Theory]
    [InlineData("species-type")]
    [InlineData("example-name")]
    [InlineData("0-retention")]
    [InlineData("1stPlace")]
    [InlineData("has space")]
    [InlineData("with.dot")]
    [InlineData("")]
    public void Given_Invalid_CSharp_Identifier_Throws(string identifier)
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdentifierException>(() =>
            Argument(type: "string", identifier: identifier));
    }

    [Theory]
    [MemberData(nameof(SupportedArgumentTypesWithDefaultValue))]
    public void Required_ScriptArgument_With_Default_Value_Compiles(string type, string defaultValue)
    {
        // Arrange
        var arg = Argument(type: type, identifier: "myParameter", required: true, defaultValue: defaultValue);

        // Act
        var syntax = arg.ToString();

        // Assert
        var formattedDefault = FormatDefaultValue(type: type, defaultValue: defaultValue);
        var expected = $"{type} myParameter = {formattedDefault}";
        Assert.Equal(expected, syntax);
    }

    [Theory]
    [MemberData(nameof(SupportedArgumentTypesWithDefaultValue))]
    public void Optional_ScriptArgument_With_Default_Value_Compiles(string type, string defaultValue)
    {
        // Arrange
        var arg = Argument(type: type, identifier: "myParameter", required: false, defaultValue: defaultValue);

        // Act
        var syntax = arg.ToString();

        // Assert
        var formattedDefault = FormatDefaultValue(type: type, defaultValue: defaultValue);
        var expected = $"{type}? myParameter = {formattedDefault}";
        Assert.Equal(expected, syntax);
    }

    [Theory]
    [ClassData(typeof(SupportedArgumentTypes))]
    public void Optional_ScriptArgument_With_Explicit_Empty_Default_Compiles(string type)
    {
        // Arrange
        var arg = Argument(type: type, identifier: "example", required: false, defaultValue: "");

        // Act
        var syntax = arg.ToString();

        // Assert
        var expected = $"{type}? example = null"; // e.g. string? example
        Assert.Equal(expected, syntax);
    }

    [Theory]
    [ClassData(typeof(SupportedArgumentTypes))]
    public void Optional_ScriptArgument_With_Explicit_NULL_Default_Compiles(string type)
    {
        // Arrange
        var arg = Argument(type: type, identifier: "example", required: false, defaultValue: "null");

        // Act
        var syntax = arg.ToString();

        // Assert
        var expected = $"{type}? example = null"; // e.g. string? example
        Assert.Equal(expected, syntax);
    }

    [Theory]
    [ClassData(typeof(SupportedArgumentTypes))]
    public void Optional_ScriptArgument_Compiles(string type)
    {
        // Arrange
        var arg = Argument(type: type, identifier: "example", required: false, defaultValue: null);

        // Act
        var syntax = arg.ToString();

        // Assert
        var expected = $"{type}? example"; // e.g. string? example
        Assert.Equal(expected, syntax);
    }

    [Theory]
    [ClassData(typeof(SupportedArgumentTypes))]
    public void Required_ScriptArgument_Compiles(string type)
    {
        // Arrange
        var arg = Argument(type: type, identifier: "name", required: true, defaultValue: null);

        // Act
        var syntax = arg.ToString();

        // Assert
        var expected = $"{type} name"; // e.g. string name
        Assert.Equal(expected, syntax);
    }

    private static ScriptArgument Argument(
        string? type = "string",
        string? identifier = "example",
        bool required = false,
        string? defaultValue = null,
        string? argName = null) =>
        ScriptArgument.Create(new ScriptArgumentOptions
        {
            Type = type,
            Identifier = identifier,
            Required = required,
            DefaultValue = defaultValue,
            ArgName = argName,
        });

    public static IEnumerable<object[]> SupportedArgumentTypesWithDefaultValue()
    {
        foreach (var type in SupportedArgumentType.All)
        {
            yield return [type, GetDefaultValue(type)];
        }
    }

    private static string GetDefaultValue(string type) =>
        type.ToLowerInvariant() switch
        {
            "string" => "example",
            "bool" => "true",
            "char" => "x",
            "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" or "float" or "double" or "decimal" => "10",
            _ => throw new NotSupportedException($"No default value mapping for type '{type}'")
        };

    private static string FormatDefaultValue(string type, string defaultValue) =>
        type.ToLowerInvariant() switch
        {
            "string" => $"\"{defaultValue}\"",
            "char" => $"'{defaultValue}'",
            "decimal" => $"{defaultValue}M",
            "float" => $"{defaultValue}F",
            "uint" => $"{defaultValue}U",
            "long" => $"{defaultValue}L",
            "ulong" => $"{defaultValue}UL",
            _ => defaultValue
        };
}
