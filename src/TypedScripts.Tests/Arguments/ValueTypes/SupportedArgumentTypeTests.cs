using TypedScripts.Arguments.Exceptions;
using TypedScripts.Arguments.ValueTypes;
using TypedScripts.Tests.Utils;
using Xunit;

namespace TypedScripts.Tests.Arguments.ValueTypes;

public class SupportedArgumentTypeTests
{
    [Theory]
    [ClassData(typeof(SupportedArgumentTypes))]
    public void Given_Supported_Type_Stores_Value(string value)
    {
        // Arrange & Act
        var type = new SupportedArgumentType(value);

        // Assert
        Assert.Equal(value, type.Value);
    }

    [Theory]
    [InlineData("INT", "int")]
    [InlineData("String", "string")]
    [InlineData("BOOL", "bool")]
    public void Supported_Type_Is_Normalized_To_Lowercase(string value, string expected)
    {
        // Arrange & Act
        var type = new SupportedArgumentType(value);

        // Assert
        Assert.Equal(expected, type.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("hobbits")]
    [InlineData("object")]
    [InlineData("var")]
    [InlineData("Int32")]
    [InlineData("system.string")]
    public void Given_Unsupported_Type_Throws(string? value)
    {
        // Arrange & Act & Assert
        Assert.Throws<UnsupportedArgumentTypeException>(() => new SupportedArgumentType(value));
    }

    [Fact]
    public void Thrown_Exception_Message_Contains_The_Unsupported_Type()
    {
        // Arrange
        const string value = "hobbits";

        // Act
        var exception = Assert.Throws<UnsupportedArgumentTypeException>(() => new SupportedArgumentType(value));

        // Assert
        Assert.Contains(value, exception.Message);
    }

    [Fact]
    public void Types_With_The_Same_Value_Are_Equal()
    {
        // Arrange
        var first = new SupportedArgumentType("int");
        var second = new SupportedArgumentType("int");

        // Act & Assert
        Assert.True(first.Equals(second));
    }

    [Fact]
    public void Types_With_Different_Values_Are_Not_Equal()
    {
        // Arrange
        var first = new SupportedArgumentType("int");
        var second = new SupportedArgumentType("string");

        // Act & Assert
        Assert.False(first.Equals(second));
    }

    [Fact]
    public void Types_Differing_Only_In_Casing_Are_Equal_After_Normalization()
    {
        // Arrange
        var lower = new SupportedArgumentType("int");
        var upper = new SupportedArgumentType("INT");

        // Act & Assert
        Assert.True(lower.Equals(upper));
    }

    [Fact]
    public void Equals_Returns_False_For_Null()
    {
        // Arrange
        var type = new SupportedArgumentType("int");

        // Act & Assert
        Assert.False(type.Equals(null));
    }

    [Fact]
    public void Equals_Object_Returns_True_For_The_Same_Value()
    {
        // Arrange
        var type = new SupportedArgumentType("int");
        object other = new SupportedArgumentType("int");

        // Act & Assert
        Assert.True(type.Equals(other));
    }

    [Fact]
    public void Equals_Object_Returns_False_For_A_Different_Type()
    {
        // Arrange
        var type = new SupportedArgumentType("int");
        object other = 42;

        // Act & Assert
        Assert.False(type.Equals(other));
    }

    [Fact]
    public void Equal_Types_Produce_Equal_Hash_Codes()
    {
        // Arrange
        var first = new SupportedArgumentType("int");
        var second = new SupportedArgumentType("int");

        // Act & Assert
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
