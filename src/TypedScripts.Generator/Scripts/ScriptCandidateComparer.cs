using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace TypedScripts.Scripts;

/// <summary>
/// Used to compare script candidates against each other.
/// Script candidates are .sh files which have not yet been parsed as an actual script.
/// </summary>
public class ScriptCandidateComparer : IEqualityComparer<(AdditionalText Text, string Content)>
{
    public static readonly ScriptCandidateComparer Instance = new();
    
    public bool Equals(
        (AdditionalText Text, string Content) x, 
        (AdditionalText Text, string Content) y) =>
        x.Text.Path.Equals(y.Text.Path) && x.Content == y.Content;

    public int GetHashCode((AdditionalText Text, string Content) obj)
    {
        unchecked
        {
            return (obj.Text.Path.GetHashCode() * 397) ^ obj.Content.GetHashCode();
        }
    }
}