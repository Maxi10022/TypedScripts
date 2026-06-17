using Microsoft.CodeAnalysis.Text;
using TypedScripts;
using TypedScripts.Common.Parser;
using TypedScripts.Scripts.Parser;
using Xunit;

namespace TypedScripts.Tests.Scripts.Parser;

public class ShebangParserTests
{
    private static ILineParseResult Parse(string line) =>
        new ShebangParser().Parse(SourceText.From(line).Lines[0]);

    private static Interpreter ParseInterpreter(string line) =>
        Assert.IsType<LineParseSuccess<Interpreter>>(Parse(line)).Value;

    private static LineParseFailure ParseFailure(string line) =>
        Assert.IsType<LineParseFailure>(Parse(line));

    [Theory]
    [InlineData("#!/bin/bash")]
    [InlineData("#!/usr/bin/bash")]
    [InlineData("#!/usr/local/bin/bash")]
    public void Parse_Returns_Bash_From_Absolute_Interpreter_Path(string line)
    {
        // Arrange & Act
        var interpreter = ParseInterpreter(line);

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Fact]
    public void Parse_Resolves_Interpreter_From_Env_Form()
    {
        // Arrange & Act
        var interpreter = ParseInterpreter("#!/usr/bin/env bash");

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Fact]
    public void Parse_Ignores_Trailing_Interpreter_Arguments()
    {
        // Arrange & Act
        var interpreter = ParseInterpreter("#!/bin/bash -e");

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Fact]
    public void Parse_Allows_Space_After_Shebang_Marker()
    {
        // Arrange & Act
        var interpreter = ParseInterpreter("#! /bin/bash");

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Theory]
    [InlineData("#!/bin/BASH")]
    [InlineData("#!/usr/bin/env BASH")]
    public void Parse_Matches_Interpreter_Name_Case_Insensitively(string line)
    {
        // Arrange & Act
        var interpreter = ParseInterpreter(line);

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Theory]
    [InlineData("#!/usr/bin/python")]
    [InlineData("#!/usr/bin/env python")]
    public void Parse_Reports_Failure_For_Unsupported_Interpreter(string line)
    {
        // Arrange & Act
        var failure = ParseFailure(line);

        // Assert
        Assert.Equal("TSC005", failure.Descriptors[0].Id);
    }

    [Fact]
    public void Parse_Skips_Line_That_Is_Not_A_Shebang()
    {
        // Arrange & Act & Assert
        Assert.IsType<SkipLineParseResult>(Parse("echo hello"));
    }

    [Fact]
    public void Parse_Skips_Plain_Comment_Line()
    {
        // Arrange & Act & Assert
        Assert.IsType<SkipLineParseResult>(Parse("# just a normal comment"));
    }

    [Fact]
    public void Parse_Skips_Shebang_Marker_That_Is_Not_At_Line_Start()
    {
        // Arrange & Act & Assert
        Assert.IsType<SkipLineParseResult>(Parse("echo #!/bin/bash"));
    }

    [Fact]
    public void Parse_Skips_Shebang_That_Is_Not_On_The_First_Line()
    {
        // Arrange
        var line = SourceText.From("# leading comment\n#!/bin/bash").Lines[1];

        // Act
        var result = new ShebangParser().Parse(line);

        // Assert
        Assert.IsType<SkipLineParseResult>(result);
    }

    [Fact]
    public void Parse_Skips_Blank_Line()
    {
        // Arrange & Act & Assert
        Assert.IsType<SkipLineParseResult>(Parse(""));
    }
}
