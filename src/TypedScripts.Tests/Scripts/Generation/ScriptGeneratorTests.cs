using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TypedScripts.Tests.Utils;
using Xunit;

namespace TypedScripts.Tests.Scripts.Generation;

public class ScriptGeneratorTests
{
    private static GeneratorDriverRunResult Run(params AdditionalText[] additionalFiles)
    {
        var generator = new IncrementalScriptGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.AddAdditionalTexts([..additionalFiles]);
        var compilation = CSharpCompilation.Create(nameof(ScriptGeneratorTests));
        return driver.RunGenerators(compilation).GetRunResult();
    }

    private static ImmutableArray<GeneratedSourceResult> GeneratedSources(GeneratorDriverRunResult result) =>
        result.Results.Single().GeneratedSources;

    private static Compilation Compile(params AdditionalText[] additionalFiles)
    {
        var generator = new IncrementalScriptGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.AddAdditionalTexts([..additionalFiles]);
        var compilation = CSharpCompilation.Create(
            nameof(ScriptGeneratorTests),
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out _);
        return output;
    }

    private const string BackupDatabaseScript =
        """
        #!/bin/bash

        # @param database:string required
        # @param outputDir:string optional default="/var/backups"
        # @param port:int optional default=5432
        # @param compress:bool optional default=true
        # @param retentionDays:int optional default=7
        # @identifier Example

        set -euo pipefail

        # Positional arguments, read in declaration order. A value is always present at
        # each index because C# passes the @param default (or "") for optional args.
        database="$1"
        output_dir="$2"
        port="$3"
        compress="$4"
        retention_days="$5"

        timestamp="$(date +%Y%m%d_%H%M%S)"
        backup_file="${output_dir}/${database}_${timestamp}.sql"

        echo "Backing up database '${database}' (port ${port})"
        echo "Target: ${backup_file}"

        if [ "$compress" = "true" ]; then
          backup_file="${backup_file}.gz"
          echo "Compression enabled -> ${backup_file}"
        fi

        echo "Pruning backups older than ${retention_days} day(s) in ${output_dir}"
        echo "Backup complete: ${backup_file}"
        """;

    [Fact]
    public void Generate_Produces_A_Single_Source_For_A_Valid_Script()
    {
        // Arrange
        var script = new TestAdditionalFile("deploy.sh", "echo deploying");

        // Act
        var result = Run(script);

        // Assert
        Assert.Single(GeneratedSources(result));
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void Generate_Embeds_The_Script_File_Contents_In_The_Generated_Source()
    {
        // Arrange
        var script = new TestAdditionalFile("deploy.sh", "echo deploying");

        // Act
        var result = Run(script);

        // Assert
        var generated = GeneratedSources(result).Single().SourceText.ToString();
        Assert.Contains("echo deploying", generated);
    }

    [Fact]
    public void Generate_Names_The_Generated_Source_After_The_Script_Identifier()
    {
        // Arrange
        var script = new TestAdditionalFile("backup.sh", "# @identifier Example\necho hi");

        // Act
        var result = Run(script);

        // Assert
        Assert.Equal("Example.g.cs", GeneratedSources(result).Single().HintName);
    }

    [Fact]
    public void Generate_Produces_A_Source_For_Each_Script_Candidate()
    {
        // Arrange
        var first = new TestAdditionalFile("alpha.sh", "echo alpha");
        var second = new TestAdditionalFile("beta.sh", "echo beta");

        // Act
        var result = Run(first, second);

        // Assert
        Assert.Equal(2, GeneratedSources(result).Length);
    }

    [Fact]
    public void Generate_Reports_Diagnostic_When_Script_Content_Cannot_Be_Read()
    {
        // Arrange
        var unreadable = new NullTextAdditionalFile("deploy.sh");

        // Act
        var result = Run(unreadable);

        // Assert
        Assert.Empty(GeneratedSources(result));
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "TSC001");
    }

    [Fact]
    public void Generate_Produces_Compilable_Source_For_A_Realistic_Script()
    {
        // Arrange
        var script = new TestAdditionalFile("backup-database.sh", BackupDatabaseScript);

        // Act
        var compilation = Compile(script);

        // Assert
        Assert.Single(compilation.SyntaxTrees);
        Assert.DoesNotContain(
            compilation.GetDiagnostics(),
            diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
    }

}
