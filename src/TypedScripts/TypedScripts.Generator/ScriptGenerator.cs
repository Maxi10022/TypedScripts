using System.Threading;
using Microsoft.CodeAnalysis;
using TypedScripts.Scripts;
using TypedScripts.Scripts.Parser;

namespace TypedScripts;

[Generator]
public class ScriptGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create incremental-value provider for script candidates.
        var scriptCandidates = context.AdditionalTextsProvider
            .Where(IsScriptCandidate)
            .Select(AsScriptCandidate)
            .WithComparer(ScriptCandidateComparer.Instance);
        
        context.RegisterSourceOutput(scriptCandidates, static (scp, scriptCandidate) =>
        {
            var result = ScriptParser.Parse(scriptCandidate.Text);
            
            if (result.HasProblems)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    scp.ReportDiagnostic(diagnostic);
                }
            }

            if (result.IsFailure) return;
            
            var script = result.Script!;
            scp.AddSource(script.FileName, script.ToString());
        });
    }

    // Can later be easily extended if other script types are to be supported.
    private static bool IsScriptCandidate(AdditionalText text) => text.Path.EndsWith(".sh");
    
    // A "Script candidate" is essentially just the AdditionalText in a tuple with the scripts content to detect changes. 
    private static (AdditionalText Text, string Content) AsScriptCandidate(
        AdditionalText text, CancellationToken ct) => 
        (Text: text, Content: text.GetText(ct)?.ToString() ?? string.Empty);
}