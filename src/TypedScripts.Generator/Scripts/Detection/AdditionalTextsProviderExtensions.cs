using System.Threading;
using Microsoft.CodeAnalysis;

namespace TypedScripts.Scripts.Detection;

public static class AdditionalTextsProviderExtensions
{
    public static IncrementalValuesProvider<ScriptCandidate> ComposeScriptCandidatesProvider(
        this IncrementalValuesProvider<AdditionalText> additionalTexts) => 
        additionalTexts
            .Where(IsScriptCandidate)
            .Select(AsScriptCandidate);

    // Can later be easily extended if other script types are to be supported.
    private static bool IsScriptCandidate(AdditionalText text) => text.Path.EndsWith(".sh");
    
    private static ScriptCandidate AsScriptCandidate(AdditionalText text, CancellationToken ct) => new(text);
}