using System.Collections.Generic;
using System.Text;
using TypedScripts.Arguments;
using TypedScripts.Common.Exceptions;
using TypedScripts.Scripts;
using TypedScripts.Scripts.Exceptions;
using Xunit;

namespace TypedScripts.Tests.Scripts;

public class ScriptTests
{
    [Fact]
    public void Create_Exposes_Identifier_Interpreters_And_Arguments()
    {
        // Arrange
        var argument = Argument("count");

        // Act
        var script = Create(identifier: "MyScript", interpreters: [Interpreter.Bash], arguments: [argument]);

        // Assert
        Assert.Equal("MyScript", script.Identifier.Value);
        Assert.Equal([Interpreter.Bash], script.Interpreters);
        Assert.Same(argument, Assert.Single(script.Arguments));
    }

    [Fact]
    public void FileName_Is_Identifier_Suffixed_With_g_cs()
    {
        // Arrange
        var script = Create(identifier: "MyScript");

        // Act
        var fileName = script.FileName;

        // Assert
        Assert.Equal("MyScriptg.cs", fileName);
    }

    [Fact]
    public void Syntax_Contains_Class_Declaration_Named_After_The_Script()
    {
        // Arrange
        var script = Create(identifier: "MyScript");

        // Act
        var generated = script.ToString();

        // Assert
        Assert.Contains("public class MyScript", generated);
    }

    [Fact]
    public void Syntax_Embeds_The_Script_Content()
    {
        // Arrange
        var script = Create(body: "echo hello world");

        // Act
        var generated = script.ToString();

        // Assert
        Assert.Contains("echo hello world", generated);
    }

    [Fact]
    public void ToString_Returns_The_Full_Generated_Syntax()
    {
        // Arrange
        var script = Create();

        // Act & Assert
        Assert.Equal(script.Syntax.ToFullString(), script.ToString());
    }

    [Theory]
    [InlineData("deploy.sh", "Deploy")]
    [InlineData("my-web-deploy.sh", "MyWebDeploy")]
    [InlineData("myWeb-deploy.sh", "MyWebDeploy")]
    [InlineData("db_backup.sh", "DbBackup")]
    [InlineData("scripts/nested.sh", "Nested")]
    public void Identifier_Falls_Back_To_PascalCased_File_Name(string filePath, string expected)
    {
        // Arrange & Act
        var script = Create(identifier: null, filePath: filePath);

        // Assert
        Assert.Equal(expected, script.Identifier.Value);
    }

    [Fact]
    public void Explicit_Identifier_Takes_Precedence_Over_File_Name()
    {
        // Arrange & Act
        var script = Create(identifier: "Explicit", filePath: "deploy.sh");

        // Assert
        Assert.Equal("Explicit", script.Identifier.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("---.sh")]
    public void Missing_Identifier_And_Underivable_File_Name_Throws(string? filePath)
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdentifierException>(() => Create(identifier: null, filePath: filePath));
    }

    [Theory]
    [InlineData("deploy.sh")]
    [InlineData("deploy.bash")]
    public void Interpreter_Is_Inferred_From_Extension(string filePath)
    {
        // Arrange & Act
        var script = Create(interpreters: [], filePath: filePath);

        // Assert
        Assert.Equal([Interpreter.Bash], script.Interpreters);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("notes.txt")]
    public void Without_Interpreters_Or_Inferable_Extension_Throws(string? filePath)
    {
        // Arrange & Act & Assert
        Assert.Throws<NoInterpreterAvailableException>(() =>
            Create(interpreters: [], filePath: filePath));
    }

    [Fact]
    public void Given_Invalid_Identifier_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidIdentifierException>(() => Create(identifier: "my-script"));
    }

    [Fact]
    public void Given_Script_Content_That_Breaks_The_Template_Throws()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidScriptSyntaxException>(() => Create(body: "\"\"\""));
    }

    private static Script Create(
        string? identifier = "MyScript",
        string? body = "echo hello",
        string? filePath = null,
        List<Interpreter>? interpreters = null,
        List<ScriptArgument>? arguments = null) =>
        Script.Create(new ScriptOptions
        {
            Identifier = identifier,
            Body = new StringBuilder(body),
            FilePath = filePath,
            Interpreters = interpreters ?? [Interpreter.Bash],
            Arguments = arguments ?? [],
        });

    private static ScriptArgument Argument(string identifier) =>
        ScriptArgument.Create(new ScriptArgumentOptions
        {
            Type = "string",
            Identifier = identifier,
            Required = true,
        });
}
