using Microsoft.CodeAnalysis.Text;
using System.Linq;
using TypedScripts.Arguments.Parser;
using Xunit;

namespace TypedScripts.Tests.Arguments.Parser;

public class ArgumentParserTests
{
    [Fact]
    public void ParseArguments_From_File_With_Single_Valid_Argument_Succeeds()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param count:int required
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal(0, arg.Position);
        Assert.Equal("int count", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Optional_Modifier_Produces_Nullable_Parameter()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param verbose:bool optional
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal(0, arg.Position);
        Assert.Equal("bool? verbose", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Omitted_Modifier_Defaults_To_Optional()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param outputFile:string
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal(0, arg.Position);
        Assert.Equal("string? outputFile", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Unquoted_Default_Value_Is_Applied()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param port:int optional default=5432
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal("int? port = 5432", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Boolean_Default_Value_Is_Applied()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param compress:bool optional default=true
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal("bool? compress = true", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Quoted_Default_Value_Strips_Surrounding_Quotes()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param outputDir:string optional default="/var/backups"
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal("string? outputDir = \"/var/backups\"", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Quoted_Default_Value_Containing_Whitespace_Is_Applied()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param message:string optional default="hello world"
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal("string? message = \"hello world\"", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Named_Argument_Sets_ArgName()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param environment:string required argName=env
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal("env", arg.ArgName);
        Assert.Equal("string environment", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Kebab_Case_Named_Argument_Sets_ArgName()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param dryRun:bool optional default=false argName=dry-run
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal("dry-run", arg.ArgName);
        Assert.Equal("bool? dryRun = false", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Modifier_Default_And_Named_Argument_Succeeds()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param retries:int optional default=3 argName=retries
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Problems);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal(0, arg.Position);
        Assert.Equal("retries", arg.ArgName);
        Assert.Equal("int? retries = 3", arg.ToString());
    }

    [Fact]
    public void ParseArguments_From_File_With_Multiple_Arguments_Assigns_Positions_In_Declaration_Order()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param first:string required
                                   # @param second:int required
                                   # @param third:bool required
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Equal(3, results.Length);
        Assert.All(results, result => Assert.True(result.IsSuccess));

        Assert.Equal(0, results[0].Argument!.Position);
        Assert.Equal("string first", results[0].Argument!.ToString());

        Assert.Equal(1, results[1].Argument!.Position);
        Assert.Equal("int second", results[1].Argument!.ToString());

        Assert.Equal(2, results[2].Argument!.Position);
        Assert.Equal("bool third", results[2].Argument!.ToString());
    }

    [Fact]
    public void ParseArguments_Ignores_Blank_Lines_Between_Definitions()
    {
        // Arrange
        var text = SourceText.From("""

                                   # @param name:string required

                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsSuccess);

        var arg = result.Argument;
        Assert.NotNull(arg);
        Assert.Equal(0, arg.Position);
        Assert.Equal("string name", arg.ToString());
    }

    [Fact]
    public void ParseArguments_With_Unsupported_Type_Reports_Failure()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param species:hobbit required
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsFailure);
        Assert.Null(result.Argument);
        Assert.Equal("TSARG001", result.Problems[0].Id);
    }

    [Fact]
    public void ParseArguments_With_Invalid_Default_Value_Reports_Failure()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param age:int optional default=gandalf
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Single(results);

        var result = results[0];
        Assert.True(result.IsFailure);
        Assert.Null(result.Argument);
        Assert.Equal("TSARG002", result.Problems[0].Id);
    }

    [Fact]
    public void ParseArguments_With_Failed_Argument_Does_Not_Consume_A_Position()
    {
        // Arrange
        var text = SourceText.From("""
                                   # @param broken:hobbit required
                                   # @param valid:string required
                                   """);

        // Act
        var results = ArgumentParser
            .ParseArguments(text)
            .ToArray();

        // Assert
        Assert.Equal(2, results.Length);

        Assert.True(results[0].IsFailure);
        Assert.Equal("TSARG001", results[0].Problems[0].Id);

        Assert.True(results[1].IsSuccess);
        Assert.Equal(0, results[1].Argument!.Position);
        Assert.Equal("string valid", results[1].Argument!.ToString());
    }
}
