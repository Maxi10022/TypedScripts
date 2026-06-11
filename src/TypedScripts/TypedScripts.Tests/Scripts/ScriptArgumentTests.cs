using TypedScripts.Scripts;
using Xunit;

namespace TypedScripts.Tests.Scripts;

public class ScriptArgumentTests
{
    [Fact]
    public void Required_ScriptArgument_With_Default_Value_Compiles()
    {
        // Arrange
        var nameArgument = new ScriptArgument(
            position: 0, 
            type: "string", 
            name: "name", 
            required: true, 
            @default: "Example"
        );

        // Act
        var syntax = nameArgument.ToString();

        // Assert
        Assert.Equal("string name = \"Example\"", syntax);
    }
    
    [Fact]
    public void Optional_ScriptArgument_Without_Default_Value_Compiles()
    {
        // Arrange
        var nameArgument = new ScriptArgument(
            position: 0, 
            type: "string", 
            name: "name", 
            required: false, 
            @default: null
        );

        // Act
        var syntax = nameArgument.ToString();

        // Assert
        Assert.Equal("string? name", syntax);
    }
    
     // TODO add more tests
}