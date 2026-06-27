using TypedScripts.Interpreters.Formatting;
using Xunit;

namespace TypedScripts.Tests.Interpreters.Interpreters;

public class BashFormatterTests
{
    private static string Escape(string value) => new BashFormatter().Escape(value);

    [Fact]
    public void Escape_Returns_Empty_Single_Quotes_For_An_Empty_String()
    {
        // Arrange & Act
        var escaped = Escape("");

        // Assert
        Assert.Equal("''", escaped);
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("HELLO")]
    [InlineData("Test123")]
    [InlineData("a")]
    [InlineData("with_underscore")]
    [InlineData("user@host")]
    [InlineData("100%")]
    [InlineData("a+b")]
    [InlineData("key=value")]
    [InlineData("key:value")]
    [InlineData("1,2,3")]
    [InlineData("file.txt")]
    [InlineData("/usr/local/bin")]
    [InlineData("--flag")]
    public void Escape_Returns_Literal_Safe_Values_Unchanged(string value)
    {
        // Arrange & Act
        var escaped = Escape(value);

        // Assert
        Assert.Equal(value, escaped);
    }

    [Theory]
    [InlineData("_")]
    [InlineData("@")]
    [InlineData("%")]
    [InlineData("+")]
    [InlineData("=")]
    [InlineData(":")]
    [InlineData(",")]
    [InlineData(".")]
    [InlineData("/")]
    [InlineData("-")]
    public void Escape_Treats_Each_Allowed_Special_Character_As_Literal_Safe(string value)
    {
        // Arrange & Act
        var escaped = Escape(value);

        // Assert
        Assert.Equal(value, escaped);
    }

    [Theory]
    [InlineData("hello world", "'hello world'")]
    [InlineData("   ", "'   '")]
    [InlineData("$HOME", "'$HOME'")]
    [InlineData("a;b", "'a;b'")]
    [InlineData("a&b", "'a&b'")]
    [InlineData("a|b", "'a|b'")]
    [InlineData("a>b", "'a>b'")]
    [InlineData("a<b", "'a<b'")]
    [InlineData("a*b", "'a*b'")]
    [InlineData("a?b", "'a?b'")]
    [InlineData("a(b)", "'a(b)'")]
    [InlineData("a{b}", "'a{b}'")]
    [InlineData("a[b]", "'a[b]'")]
    [InlineData("a`b`", "'a`b`'")]
    [InlineData("a#b", "'a#b'")]
    [InlineData("a!b", "'a!b'")]
    [InlineData("a~b", "'a~b'")]
    [InlineData("tab\tchar", "'tab\tchar'")]
    [InlineData("new\nline", "'new\nline'")]
    public void Escape_Single_Quotes_Values_Containing_Unsafe_Characters(string value, string expected)
    {
        // Arrange & Act
        var escaped = Escape(value);

        // Assert
        Assert.Equal(expected, escaped);
    }

    [Fact]
    public void Escape_Single_Quotes_A_Value_Containing_A_Backslash()
    {
        // Arrange & Act
        var escaped = Escape(@"back\slash");

        // Assert
        Assert.Equal(@"'back\slash'", escaped);
    }

    [Fact]
    public void Escape_Single_Quotes_A_Value_Containing_A_Double_Quote()
    {
        // Arrange & Act
        var escaped = Escape("say \"hi\"");

        // Assert
        Assert.Equal("'say \"hi\"'", escaped);
    }

    [Fact]
    public void Escape_Closes_Reopens_And_Backslash_Escapes_An_Embedded_Single_Quote()
    {
        // Arrange & Act
        var escaped = Escape("it's");

        // Assert
        Assert.Equal(@"'it'\''s'", escaped);
    }

    [Fact]
    public void Escape_Handles_A_Value_That_Is_Only_A_Single_Quote()
    {
        // Arrange & Act
        var escaped = Escape("'");

        // Assert
        Assert.Equal(@"''\'''", escaped);
    }

    [Fact]
    public void Escape_Escapes_Every_Single_Quote_In_A_Value()
    {
        // Arrange & Act
        var escaped = Escape("a'b'c");

        // Assert
        Assert.Equal(@"'a'\''b'\''c'", escaped);
    }

    [Fact]
    public void Escape_Of_A_Single_Quoted_Value_Round_Trips_Back_To_The_Original()
    {
        // Arrange
        const string value = "rm -rf 'a b'; echo $PATH";

        // Act
        var escaped = Escape(value);

        // Assert
        Assert.Equal(value, Unquote(escaped));
    }

    // Reverses Bash single-quote escaping: strips the wrapping quotes and turns
    // each '\'' sequence back into a literal single quote.
    private static string Unquote(string escaped)
    {
        Assert.StartsWith("'", escaped);
        Assert.EndsWith("'", escaped);
        var inner = escaped.Substring(1, escaped.Length - 2);
        return inner.Replace(@"'\''", "'");
    }
}
