using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using TypedScripts.Scripts;
using TypedScripts.Tests.Utils;
using Xunit;

namespace TypedScripts.Tests.Scripts;

public class ScriptCandidateComparerTests
{
    private static (AdditionalText Text, string Content) Candidate(string path, string content) =>
        (new TestAdditionalFile(path, content), content);

    [Fact]
    public void Same_Path_And_Content_Are_Equal()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("backup.sh", "echo hi");

        // Act & Assert
        Assert.True(ScriptCandidateComparer.Instance.Equals(x, y));
    }

    [Fact]
    public void Different_Instances_With_Same_Path_And_Content_Are_Equal()
    {
        // Arrange: two distinct AdditionalText instances — equality is by path string, not reference.
        var text1 = new TestAdditionalFile("backup.sh", "echo hi");
        var text2 = new TestAdditionalFile("backup.sh", "echo hi");

        // Act & Assert
        Assert.NotSame(text1, text2);
        Assert.True(ScriptCandidateComparer.Instance.Equals((text1, "echo hi"), (text2, "echo hi")));
    }

    [Fact]
    public void Same_Path_Different_Content_Are_Not_Equal()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("backup.sh", "echo bye");

        // Act & Assert
        Assert.False(ScriptCandidateComparer.Instance.Equals(x, y));
    }

    [Fact]
    public void Different_Path_Same_Content_Are_Not_Equal()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("restore.sh", "echo hi");

        // Act & Assert
        Assert.False(ScriptCandidateComparer.Instance.Equals(x, y));
    }

    [Fact]
    public void Different_Path_And_Content_Are_Not_Equal()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("restore.sh", "echo bye");

        // Act & Assert
        Assert.False(ScriptCandidateComparer.Instance.Equals(x, y));
    }

    [Fact]
    public void Equal_Candidates_Produce_Equal_Hash_Codes()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("backup.sh", "echo hi");

        // Act & Assert: the mandatory Equals/GetHashCode contract.
        Assert.Equal(
            ScriptCandidateComparer.Instance.GetHashCode(x),
            ScriptCandidateComparer.Instance.GetHashCode(y));
    }

    [Fact]
    public void Differing_Content_Changes_The_Hash_Code()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("backup.sh", "echo bye");

        // Act & Assert: guards against a refactor that drops Content from the hash.
        Assert.NotEqual(
            ScriptCandidateComparer.Instance.GetHashCode(x),
            ScriptCandidateComparer.Instance.GetHashCode(y));
    }

    [Fact]
    public void Differing_Path_Changes_The_Hash_Code()
    {
        // Arrange
        var x = Candidate("backup.sh", "echo hi");
        var y = Candidate("restore.sh", "echo hi");

        // Act & Assert
        Assert.NotEqual(
            ScriptCandidateComparer.Instance.GetHashCode(x),
            ScriptCandidateComparer.Instance.GetHashCode(y));
    }

    [Fact]
    public void Distinct_Collapses_Duplicate_Candidates()
    {
        // Arrange: the scenario the comparer exists for — dedup across (Equals + GetHashCode).
        var candidates = new[]
        {
            Candidate("backup.sh", "echo hi"),
            Candidate("backup.sh", "echo hi"),   // duplicate of the first
            Candidate("backup.sh", "echo bye"),  // same path, edited content
            Candidate("restore.sh", "echo hi"),  // different path
        };

        // Act
        var distinct = candidates.Distinct(ScriptCandidateComparer.Instance).ToList();

        // Assert
        Assert.Equal(3, distinct.Count);
    }

    [Fact]
    public void HashSet_Treats_Same_Path_And_Content_As_One_Entry()
    {
        // Arrange
        var set = new HashSet<(AdditionalText Text, string Content)>(ScriptCandidateComparer.Instance);

        // Act
        var addedFirst = set.Add(Candidate("backup.sh", "echo hi"));
        var addedSecond = set.Add(Candidate("backup.sh", "echo hi"));

        // Assert
        Assert.True(addedFirst);
        Assert.False(addedSecond);
        Assert.Single(set);
    }
}
