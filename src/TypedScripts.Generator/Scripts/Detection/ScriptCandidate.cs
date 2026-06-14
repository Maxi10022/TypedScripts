using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypedScripts.Scripts.Detection;

/// <summary>
/// Represents a detected additional file where its file extension suggests it's a supported script.   
/// </summary>
/// <param name="additionalText">Actual script.</param>
public class ScriptCandidate(AdditionalText additionalText) : IEquatable<ScriptCandidate>
{
    public string Path { get; } = additionalText.Path;
    public SourceText? SourceText { get; } = additionalText.GetText();
    private readonly string? _content = additionalText.GetText()?.ToString();

    public bool Equals(ScriptCandidate? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _content == other._content && Path == other.Path;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ScriptCandidate)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((_content != null ? _content.GetHashCode() : 0) * 397) ^ Path.GetHashCode();
        }
    }
}