using Microsoft.CodeAnalysis.Text;
using TypedScripts.Arguments;
using TypedScripts.Arguments.Parser;
using TypedScripts.Common.Parser;
using Xunit;

namespace TypedScripts.Tests.Arguments.Parser;

public class ArgumentParserTests
{
    private static ILineParseResult Parse(string line) =>
        new ArgumentParser().Parse(SourceText.From(line).Lines[0]);

    private static ScriptArgument ParseArgument(string line) =>
        Assert.IsType<LineParseSuccess<ScriptArgument>>(Parse(line)).Value;

    private static LineParseFailure ParseFailure(string line) =>
        Assert.IsType<LineParseFailure>(Parse(line));

    [Fact]
    public void Parse_Required_Argument_Produces_Non_Nullable_Parameter()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param count:int required");

        // Assert
        Assert.Equal("int count", arg.ToString());
    }

    [Fact]
    public void Parse_Optional_Argument_Produces_Nullable_Parameter()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param verbose:bool optional");

        // Assert
        Assert.Equal("bool? verbose", arg.ToString());
    }

    [Fact]
    public void Parse_With_Omitted_Modifier_Defaults_To_Optional()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param outputFile:string");

        // Assert
        Assert.Equal("string? outputFile", arg.ToString());
    }

    [Fact]
    public void Parse_Applies_Unquoted_Default_Value()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param port:int optional default=5432");

        // Assert
        Assert.Equal("int? port = 5432", arg.ToString());
    }

    [Fact]
    public void Parse_Applies_Boolean_Default_Value()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param compress:bool optional default=true");

        // Assert
        Assert.Equal("bool? compress = true", arg.ToString());
    }

    [Fact]
    public void Parse_Strips_Surrounding_Quotes_From_Default_Value()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param outputDir:string optional default=\"/var/backups\"");

        // Assert
        Assert.Equal("string? outputDir = \"/var/backups\"", arg.ToString());
    }

    [Fact]
    public void Parse_Preserves_Whitespace_In_Quoted_Default_Value()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param message:string optional default=\"hello world\"");

        // Assert
        Assert.Equal("string? message = \"hello world\"", arg.ToString());
    }

    [Fact]
    public void Parse_Sets_ArgName_For_Named_Argument()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param environment:string required argName=env");

        // Assert
        Assert.Equal("env", arg.ArgName);
        Assert.Equal("string environment", arg.ToString());
    }

    [Fact]
    public void Parse_Sets_Kebab_Case_ArgName()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param dryRun:bool optional default=false argName=dry-run");

        // Assert
        Assert.Equal("dry-run", arg.ArgName);
        Assert.Equal("bool? dryRun = false", arg.ToString());
    }

    [Fact]
    public void Parse_With_Modifier_Default_And_Named_Argument_Succeeds()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param retries:int optional default=3 argName=retries");

        // Assert
        Assert.Equal("retries", arg.ArgName);
        Assert.Equal("int? retries = 3", arg.ToString());
    }

    [Fact]
    public void Parse_Skips_Line_That_Is_Not_A_Param_Definition()
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
    public void Parse_With_Unsupported_Type_Reports_Failure()
    {
        // Arrange & Act
        var failure = ParseFailure("# @param species:hobbit required");

        // Assert
        Assert.Equal("TSARG001", failure.Descriptors[0].Id);
    }

    [Fact]
    public void Parse_With_Invalid_Default_Value_Reports_Failure()
    {
        // Arrange & Act
        var failure = ParseFailure("# @param age:int optional default=gandalf");

        // Assert
        Assert.Equal("TSARG002", failure.Descriptors[0].Id);
    }

    [Theory]
    [InlineData("example-name")]
    [InlineData("0-retention")]
    [InlineData("with.dot")]
    [InlineData("1stPlace")]
    public void Parse_With_Invalid_CSharp_Identifier_Reports_Failure(string identifier)
    {
        // Arrange & Act
        var failure = ParseFailure($"# @param {identifier}:string required");

        // Assert
        Assert.Equal("TSARG003", failure.Descriptors[0].Id);
    }

    [Fact]
    public void Parse_Allows_Modifier_After_Default_Value()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param port:int default=5432 optional");

        // Assert
        Assert.Equal("int? port = 5432", arg.ToString());
    }

    [Fact]
    public void Parse_Allows_ArgName_Before_Modifier()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param environment:string argName=env required");

        // Assert
        Assert.Equal("env", arg.ArgName);
        Assert.Equal("string environment", arg.ToString());
    }

    [Fact]
    public void Parse_Allows_Options_In_Any_Order()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param retries:int argName=retries default=3 optional");

        // Assert
        Assert.Equal("retries", arg.ArgName);
        Assert.Equal("int? retries = 3", arg.ToString());
    }

    [Fact]
    public void Parse_Preserves_Quoted_Default_Regardless_Of_Position()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param message:string argName=msg default=\"hello world\" optional");

        // Assert
        Assert.Equal("msg", arg.ArgName);
        Assert.Equal("string? message = \"hello world\"", arg.ToString());
    }

    [Fact]
    public void Parse_Treats_Modifier_Case_Insensitively()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param count:int REQUIRED");

        // Assert
        Assert.Equal("int count", arg.ToString());
    }

    [Fact]
    public void Parse_Ignores_Unrecognized_Tokens()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param count:int required nonsense");

        // Assert
        Assert.Equal("int count", arg.ToString());
    }

    [Fact]
    public void Parse_With_Only_Unrecognized_Trailing_Tokens_Defaults_To_Optional()
    {
        // Arrange & Act
        var arg = ParseArgument("# @param count:int blah");

        // Assert
        Assert.Equal("int? count", arg.ToString());
    }
}
