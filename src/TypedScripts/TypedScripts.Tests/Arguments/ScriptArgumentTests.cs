using System;
using System.Collections.Generic;
using TypedScripts.Arguments;
using TypedScripts.Arguments.Exceptions;
using Xunit;

namespace TypedScripts.Tests.Arguments;

public class ScriptArgumentTests
{
    [Fact]
    public void Using_Unsupported_Type_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<UnsupportedArgumentTypeException>(() =>
        {
            _ = new ScriptArgument(
                position: 0,
                type: "hobbits",
                name: "species",
                lineNumber: 0,
                required: false,
                defaultValue: null,
                argName: null
            );
        });
    }
    
    [Fact]
    public void Using_Invalid_Default_Value_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidArgumentDefaultException>(() =>
        {
            _ = new ScriptArgument(
                position: 0,
                type: "int",
                name: "species",
                lineNumber: 0,
                required: false,
                defaultValue: "gandalf",
                argName: null
            );
        });
    }

    [Fact]
    public void Given_Invalid_CSharp_Identifier_Name_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidParameterIdentifierException>(() =>
        {
            var x = new ScriptArgument(
                position: 0,
                type: "string",
                name: "species-type",
                lineNumber: 0,
                required: false,
                defaultValue: "dwarf",
                argName: null
            );
        });
    }
    
    [Theory]
    [MemberData(nameof(SupportedArgumentTypesWithDefaultValue))]
    public void Required_ScriptArgument_With_Default_Value_Compiles(string type, string defaultValue)
    {
        // Arrange
        var arg = new ScriptArgument(
            position: 0, 
            type: type,
            name: "myParameter", 
            lineNumber: 0,
            required: true, 
            defaultValue: defaultValue,
            argName: null
        );

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
        var arg = new ScriptArgument(
            position: 0, 
            type: type,
            name: "myParameter", 
            lineNumber: 0,
            required: false, 
            defaultValue: defaultValue,
            argName: null
        );

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
        var arg = new ScriptArgument(
            position: 0, 
            type: type, 
            name: "example", 
            lineNumber: 0,
            required: false, 
            defaultValue: "",
            argName: null
        );

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
        var arg = new ScriptArgument(
            position: 0, 
            type: type, 
            name: "example", 
            lineNumber: 0,
            required: false, 
            defaultValue: "null",
            argName: null
        );

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
        var arg = new ScriptArgument(
            position: 0, 
            type: type, 
            name: "example", 
            lineNumber: 0,
            required: false, 
            defaultValue: null,
            argName: null
        );

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
        var arg = new ScriptArgument(
            position: 0, 
            type: type, 
            name: "name", 
            lineNumber: 0,
            required: true, 
            defaultValue: null,
            argName: null
        );

        // Act
        var syntax = arg.ToString();

        // Assert
        var expected = $"{type} name"; // e.g. string name
        Assert.Equal(expected, syntax);
    }

    // Test data setup 
    public static IEnumerable<object[]> SupportedArgumentTypesWithDefaultValue()
    {
        foreach (var type in TypedScripts.Arguments.SupportedArgumentTypes.All)
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