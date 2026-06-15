using TypedScripts.Arguments;
using TypedScripts.Scripts;
using TypedScripts.Scripts.Exceptions;
using Xunit;

namespace TypedScripts.Tests.Scripts;

public class ScriptTests
{
    [Fact]
    public void Constructor_Exposes_Name_Shell_And_Arguments()
    {
        // Arrange
        var arguments = new[] { Argument("count") };

        // Act
        var script = new Script(
            name: "MyScript", 
            scriptContent: "echo hello", 
            interpreter: Interpreter.Bash, 
            arguments: arguments
        );

        // Assert
        Assert.Equal("MyScript", script.Name);
        Assert.Equal(Interpreter.Bash, script.Interpreter);
        Assert.Same(arguments, script.Arguments);
    }

    [Fact]
    public void FileName_Is_Name_Suffixed_With_g_cs()
    {
        // Arrange
        var script = new Script(
            name: "MyScript", 
            scriptContent: "echo hello", 
            interpreter: Interpreter.Bash, 
            arguments: []
        );

        // Act
        var fileName = script.FileName;

        // Assert
        Assert.Equal("MyScriptg.cs", fileName);
    }

    [Fact]
    public void Syntax_Contains_Class_Declaration_Named_After_The_Script()
    {
        // Arrange
        var script = new Script(
            name: "MyScript", 
            scriptContent: "echo hello", 
            interpreter: Interpreter.Bash, 
            arguments: []
        );

        // Act
        var generated = script.ToString();

        // Assert
        Assert.Contains("public class MyScript", generated);
    }

    [Fact]
    public void Syntax_Embeds_The_Script_Content()
    {
        // Arrange
        var script = new Script(
            name: "MyScript", 
            scriptContent: "echo hello world",
            interpreter: Interpreter.Bash, 
            arguments: []
        );

        // Act
        var generated = script.ToString();

        // Assert
        Assert.Contains("echo hello world", generated);
    }

    [Fact]
    public void ToString_Returns_The_Full_Generated_Syntax()
    {
        // Arrange
        var script = new Script(
            name: "MyScript", 
            scriptContent: "echo hello", 
            interpreter: Interpreter.Bash, 
            arguments: []
        );

        // Act & Assert
        Assert.Equal(script.Syntax.ToFullString(), script.ToString());
    }

    [Fact]
    public void Given_Script_Content_That_Breaks_The_Template_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidScriptSyntaxException>(() =>
            new Script(name: "MyScript", scriptContent: "\"\"\"", interpreter: Interpreter.Bash, arguments: []));
    }

    private static ScriptArgument Argument(string name) => new(
        position: 0, 
        type: "string", 
        identifier: name, 
        lineNumber: 0, 
        required: true, 
        defaultValue: null, 
        argName: null
    );
}
