using System.Linq;
using TypedScripts.Scripts.Detection;
using TypedScripts.Tests.Utils;
using Xunit;

namespace TypedScripts.Tests.Scripts.Detection;

public class ScriptCandidateTests
{
    private static ScriptCandidate Candidate(string path, string content) =>
        new(new TestAdditionalFile(path, content));

    [Fact]
    public void Unchanged_Script_Is_Equal()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("backup.sh", "echo hi");

        // Act & Assert
        Assert.True(current.Equals(previous));
    }

    [Fact]
    public void Unchanged_Script_From_A_Fresh_AdditionalText_Instance_Stays_Equal()
    {
        // Arrange
        var first = new TestAdditionalFile("backup.sh", "echo hi");
        var second = new TestAdditionalFile("backup.sh", "echo hi");

        // Act & Assert
        Assert.NotSame(first, second);
        Assert.True(new ScriptCandidate(first).Equals(new ScriptCandidate(second)));
    }

    [Fact]
    public void Changed_Content_Is_Detected_As_Not_Equal()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("backup.sh", "echo bye");

        // Act & Assert
        Assert.False(current.Equals(previous));
    }

    [Fact]
    public void Renamed_Script_With_Same_Content_Is_Not_Equal()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("restore.sh", "echo hi");

        // Act & Assert
        Assert.False(current.Equals(previous));
    }

    [Fact]
    public void Different_Path_And_Content_Are_Not_Equal()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("restore.sh", "echo bye");

        // Act & Assert
        Assert.False(current.Equals(previous));
    }

    [Fact]
    public void Unchanged_Script_Produces_Equal_Hash_Codes()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("backup.sh", "echo hi");

        // Act & Assert
        Assert.Equal(previous.GetHashCode(), current.GetHashCode());
    }

    [Fact]
    public void Changed_Content_Changes_The_Hash_Code()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("backup.sh", "echo bye");

        // Act & Assert
        Assert.NotEqual(previous.GetHashCode(), current.GetHashCode());
    }

    [Fact]
    public void Different_Path_Changes_The_Hash_Code()
    {
        // Arrange
        var previous = Candidate("backup.sh", "echo hi");
        var current = Candidate("restore.sh", "echo hi");

        // Act & Assert
        Assert.NotEqual(previous.GetHashCode(), current.GetHashCode());
    }

    [Fact]
    public void Distinct_Collapses_Unchanged_Candidates()
    {
        // Arrange
        var candidates = new[]
        {
            Candidate("backup.sh", "echo hi"),
            Candidate("backup.sh", "echo hi"),
            Candidate("backup.sh", "echo bye"),
            Candidate("restore.sh", "echo hi"),
        };

        // Act
        var distinct = candidates.Distinct().ToList();

        // Assert
        Assert.Equal(3, distinct.Count);
    }

    [Fact]
    public void Equals_Object_Returns_True_For_The_Same_Candidate()
    {
        // Arrange
        var candidate = Candidate("backup.sh", "echo hi");
        object other = Candidate("backup.sh", "echo hi");

        // Act & Assert
        Assert.True(candidate.Equals(other));
    }

    [Fact]
    public void Equals_Object_Returns_False_For_A_Different_Type()
    {
        // Arrange
        var candidate = Candidate("backup.sh", "echo hi");
        object other = 42;

        // Act & Assert
        Assert.False(candidate.Equals(other));
    }

    [Fact]
    public void Equals_Returns_False_For_Null()
    {
        // Arrange
        var candidate = Candidate("backup.sh", "echo hi");

        // Act & Assert
        Assert.False(candidate.Equals(null));
    }

    [Fact]
    public void Path_Is_Taken_From_The_Additional_File()
    {
        // Arrange & Act
        var candidate = Candidate("backup.sh", "echo hi");

        // Assert
        Assert.Equal("backup.sh", candidate.Path);
    }

    [Fact]
    public void SourceText_Exposes_The_Script_Body()
    {
        // Arrange & Act
        var candidate = Candidate("backup.sh", "echo hi");

        // Assert
        Assert.Equal("echo hi", candidate.SourceText?.ToString());
    }
}
