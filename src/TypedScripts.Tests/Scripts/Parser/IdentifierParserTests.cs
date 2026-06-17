using Microsoft.CodeAnalysis.Text;
using TypedScripts.Common;
using TypedScripts.Common.Parser;
using TypedScripts.Scripts.Parser;
using Xunit;

namespace TypedScripts.Tests.Scripts.Parser;

public class IdentifierParserTests
{
    private static ILineParseResult Parse(string line) =>
        new IdentifierParser().Parse(SourceText.From(line).Lines[0]);

    private static SafeIdentifier ParseIdentifier(string line) =>
        Assert.IsType<LineParseSuccess<SafeIdentifier>>(Parse(line)).Value;

    private static LineParseFailure ParseFailure(string line) =>
        Assert.IsType<LineParseFailure>(Parse(line));

    [Fact]
    public void Parse_Returns_Identifier_From_Hash_Comment()
    {
        // Arrange & Act
        var identifier = ParseIdentifier("# @identifier BackupDatabase");

        // Assert
        Assert.Equal("BackupDatabase", identifier.Value);
    }

    [Theory]
    [InlineData("// @identifier BackupDatabase")]
    [InlineData("-- @identifier BackupDatabase")]
    [InlineData("; @identifier BackupDatabase")]
    [InlineData("% @identifier BackupDatabase")]
    [InlineData(":: @identifier BackupDatabase")]
    [InlineData("REM @identifier BackupDatabase")]
    public void Parse_Recognizes_Identifier_For_Each_Comment_Leader(string line)
    {
        // Arrange & Act
        var identifier = ParseIdentifier(line);

        // Assert
        Assert.Equal("BackupDatabase", identifier.Value);
    }

    [Theory]
    [InlineData("# @IDENTIFIER BackupDatabase")]
    [InlineData("# @Identifier BackupDatabase")]
    public void Parse_Matches_Directive_Case_Insensitively(string line)
    {
        // Arrange & Act
        var identifier = ParseIdentifier(line);

        // Assert
        Assert.Equal("BackupDatabase", identifier.Value);
    }

    [Fact]
    public void Parse_Allows_Leading_Indentation()
    {
        // Arrange & Act
        var identifier = ParseIdentifier("    # @identifier BackupDatabase");

        // Assert
        Assert.Equal("BackupDatabase", identifier.Value);
    }

    [Fact]
    public void Parse_Escapes_Reserved_Keyword_Identifier()
    {
        // Arrange & Act
        var identifier = ParseIdentifier("# @identifier class");

        // Assert
        Assert.Equal("@class", identifier.Value);
    }

    [Theory]
    [InlineData("# @identifier 1stPlace")]
    [InlineData("# @identifier my-script")]
    [InlineData("# @identifier with.dot")]
    public void Parse_Reports_Failure_For_Invalid_Identifier(string line)
    {
        // Arrange & Act
        var failure = ParseFailure(line);

        // Assert
        Assert.Equal("TSC002", failure.Descriptors[0].Id);
    }

    [Fact]
    public void Parse_Skips_Line_That_Is_Not_An_Identifier_Definition()
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
        Assert.IsType<SkipLineParseResult>(Parse("# @identifier"));
    }
}
