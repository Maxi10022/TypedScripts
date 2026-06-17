using System.Threading;
using Microsoft.CodeAnalysis;
using TypedScripts.Scripts;
using TypedScripts.Scripts.Detection;
using TypedScripts.Scripts.Generation;

namespace TypedScripts;

[Generator]
public class IncrementalScriptGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var scriptGenerator = new ScriptGenerator();
        var scriptCandidates = context.AdditionalTextsProvider.ComposeScriptCandidatesProvider();
        context.RegisterSourceOutput(scriptCandidates, scriptGenerator.Generate);
    }
}