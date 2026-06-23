using System.Text.RegularExpressions;
using TypedScripts.Arguments.Parser;
using Xunit;

namespace TypedScripts.Tests.Arguments.Parser;

public class RegexMatchExtensionsTests
{
    // Produces a Match whose Value is the whole input, mirroring a single token
    // as emitted by the parser's token pattern.
    private static Match Token(string value) => Regex.Match(value, ".+");

    [Fact]
    public void GetTokenValue_Returns_Value_After_Prefix()
    {
        // Arrange
        var token = Token("default=5432");

        // Act
        var value = token.GetTokenValue("default=");

        // Assert
        Assert.Equal("5432", value);
    }

    [Fact]
    public void GetTokenValue_Strips_Surrounding_Quotes()
    {
        // Arrange
        var token = Token("default=\"hello world\"");

        // Act
        var value = token.GetTokenValue("default=");

        // Assert
        Assert.Equal("hello world", value);
    }

    [Fact]
    public void GetTokenValue_Returns_Empty_String_For_Empty_Quotes()
    {
        // Arrange
        var token = Token("default=\"\"");

        // Act
        var value = token.GetTokenValue("default=");

        // Assert
        Assert.Equal("", value);
    }

    [Fact]
    public void GetTokenValue_Leaves_Unquoted_Value_Unchanged()
    {
        // Arrange
        var token = Token("argName=dry-run");

        // Act
        var value = token.GetTokenValue("argName=");

        // Assert
        Assert.Equal("dry-run", value);
    }

    [Fact]
    public void GetTokenValue_Does_Not_Strip_When_Quotes_Are_Unbalanced()
    {
        // Arrange
        var token = Token("default=5\"");

        // Act
        var value = token.GetTokenValue("default=");

        // Assert
        Assert.Equal("5\"", value);
    }

    [Fact]
    public void ValueEquals_Returns_True_For_Identical_Value()
    {
        // Arrange & Act & Assert
        Assert.True(Token("required").ValueEquals("required"));
    }

    [Fact]
    public void ValueEquals_Ignores_Case()
    {
        // Arrange & Act & Assert
        Assert.True(Token("REQUIRED").ValueEquals("required"));
    }

    [Fact]
    public void ValueEquals_Returns_False_For_Different_Value()
    {
        // Arrange & Act & Assert
        Assert.False(Token("optional").ValueEquals("required"));
    }

    [Fact]
    public void StartsWith_Returns_True_When_Token_Has_Prefix()
    {
        // Arrange & Act & Assert
        Assert.True(Token("default=5").StartsWith("default="));
    }

    [Fact]
    public void StartsWith_Ignores_Case()
    {
        // Arrange & Act & Assert
        Assert.True(Token("DEFAULT=5").StartsWith("default="));
    }

    [Fact]
    public void StartsWith_Returns_False_When_Prefix_Absent()
    {
        // Arrange & Act & Assert
        Assert.False(Token("argName=foo").StartsWith("default="));
    }
}
