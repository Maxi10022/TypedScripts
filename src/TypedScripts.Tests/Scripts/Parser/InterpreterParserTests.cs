using Microsoft.CodeAnalysis.Text;
using TypedScripts;
using TypedScripts.Common.Parser;
using TypedScripts.Scripts.Parser;
using Xunit;

namespace TypedScripts.Tests.Scripts.Parser;

public class InterpreterParserTests
{
    private static ILineParseResult Parse(string line) =>
        new InterpreterParser().Parse(SourceText.From(line).Lines[0]);

    private static Interpreter ParseInterpreter(string line) =>
        Assert.IsType<LineParseSuccess<Interpreter>>(Parse(line)).Value;

    private static LineParseFailure ParseFailure(string line) =>
        Assert.IsType<LineParseFailure>(Parse(line));

    [Fact]
    public void Parse_Returns_Bash_From_Interpreter_Directive()
    {
        // Arrange & Act
        var interpreter = ParseInterpreter("# @interpreter bash");

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Theory]
    [InlineData("# @interpreter bash")]
    [InlineData("# @interpreter BASH")]
    [InlineData("# @interpreter Bash")]
    public void Parse_Matches_Interpreter_Name_Case_Insensitively(string line)
    {
        // Arrange & Act
        var interpreter = ParseInterpreter(line);

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Theory]
    [InlineData("// @interpreter bash")]
    [InlineData("-- @interpreter bash")]
    [InlineData("; @interpreter bash")]
    [InlineData("% @interpreter bash")]
    [InlineData(":: @interpreter bash")]
    [InlineData("REM @interpreter bash")]
    public void Parse_Recognizes_Interpreter_For_Each_Comment_Leader(string line)
    {
        // Arrange & Act
        var interpreter = ParseInterpreter(line);

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Theory]
    [InlineData("# @INTERPRETER bash")]
    [InlineData("# @Interpreter bash")]
    public void Parse_Matches_Directive_Case_Insensitively(string line)
    {
        // Arrange & Act
        var interpreter = ParseInterpreter(line);

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Fact]
    public void Parse_Allows_Leading_Indentation()
    {
        // Arrange & Act
        var interpreter = ParseInterpreter("    # @interpreter bash");

        // Assert
        Assert.Equal(Interpreter.Bash, interpreter);
    }

    [Theory]
    [InlineData("# @interpreter python")]
    [InlineData("# @interpreter powershell")]
    public void Parse_Reports_Failure_For_Unsupported_Interpreter(string line)
    {
        // Arrange & Act
        var failure = ParseFailure(line);

        // Assert
        Assert.Equal("TSC005", failure.Descriptors[0].Id);
    }

    [Fact]
    public void Parse_Skips_Line_That_Is_Not_An_Interpreter_Directive()
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
    public void Parse_Skips_Blank_Line()
    {
        // Arrange & Act & Assert
        Assert.IsType<SkipLineParseResult>(Parse(""));
    }

    [Fact]
    public void Parse_Skips_Directive_Without_A_Value()
    {
        // Arrange & Act & Assert
        Assert.IsType<SkipLineParseResult>(Parse("# @interpreter"));
    }
}
