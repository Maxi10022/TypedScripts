using TypedScripts.Common;
using TypedScripts.Common.Exceptions;
using Xunit;

namespace TypedScripts.Tests.Common;

public class SafeIdentifierTests
{
    [Theory]
    [InlineData("Backup")]
    [InlineData("MyScript")]
    [InlineData("_private")]
    [InlineData("_")]
    [InlineData("script_1")]
    [InlineData("a")]
    public void Given_Valid_Identifier_Stores_Value(string value)
    {
        // Arrange & Act
        var className = new SafeIdentifier(value);

        // Assert
        Assert.Equal(value, className.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("1script")]
    [InlineData("0-retention")]
    [InlineData("my-script")]
    [InlineData("has space")]
    [InlineData("with.dot")]
    public void Given_Invalid_Identifier_Throws(string? value)
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdentifierException>(() => new SafeIdentifier(value));
    }

    [Fact]
    public void Thrown_Exception_Message_Contains_The_Invalid_Name()
    {
        // Arrange
        const string value = "my-script";

        // Act
        var exception = Assert.Throws<InvalidIdentifierException>(() => new SafeIdentifier(value));

        // Assert
        Assert.Contains(value, exception.Message);
    }

    [Fact]
    public void Implicit_Conversion_From_Valid_String_Stores_Value()
    {
        // Arrange & Act
        SafeIdentifier identifier = "MyScript";

        // Assert
        Assert.Equal("MyScript", identifier.Value);
    }

    [Fact]
    public void Implicit_Conversion_From_Invalid_String_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdentifierException>(() =>
        {
            SafeIdentifier _ = "my-script";
        });
    }

    [Fact]
    public void ToString_Returns_The_Value_For_A_Regular_Identifier()
    {
        // Arrange
        var identifier = new SafeIdentifier("MyScript");

        // Act
        var text = identifier.ToString();

        // Assert
        Assert.Equal("MyScript", text);
    }

    [Fact]
    public void ToString_Escapes_A_Reserved_Keyword_With_An_At_Sign()
    {
        // Arrange
        var identifier = new SafeIdentifier("class");

        // Act
        var text = identifier.ToString();

        // Assert
        Assert.Equal("class", identifier.Value);
        Assert.Equal("@class", text);
    }

    [Fact]
    public void Identifiers_With_The_Same_Value_Are_Equal()
    {
        // Arrange
        var first = new SafeIdentifier("MyScript");
        var second = new SafeIdentifier("MyScript");

        // Act & Assert
        Assert.True(first.Equals(second));
    }

    [Fact]
    public void Identifiers_With_Different_Values_Are_Not_Equal()
    {
        // Arrange
        var first = new SafeIdentifier("MyScript");
        var second = new SafeIdentifier("OtherScript");

        // Act & Assert
        Assert.False(first.Equals(second));
    }

    [Fact]
    public void Equals_Returns_False_For_Null()
    {
        // Arrange
        var identifier = new SafeIdentifier("MyScript");

        // Act & Assert
        Assert.False(identifier.Equals(null));
    }

    [Fact]
    public void Equals_Object_Returns_True_For_The_Same_Value()
    {
        // Arrange
        var identifier = new SafeIdentifier("MyScript");
        object other = new SafeIdentifier("MyScript");

        // Act & Assert
        Assert.True(identifier.Equals(other));
    }

    [Fact]
    public void Equals_Object_Returns_False_For_A_Different_Type()
    {
        // Arrange
        var identifier = new SafeIdentifier("MyScript");
        object other = 42;

        // Act & Assert
        Assert.False(identifier.Equals(other));
    }

    [Fact]
    public void Equal_Identifiers_Produce_Equal_Hash_Codes()
    {
        // Arrange
        var first = new SafeIdentifier("MyScript");
        var second = new SafeIdentifier("MyScript");

        // Act & Assert
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
