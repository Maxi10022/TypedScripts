using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace TypedScripts.Tests.Utils;

public class NullTextAdditionalFile(string path) : AdditionalText
{
    public override SourceText? GetText(CancellationToken cancellationToken = new()) => null;

    public override string Path { get; } = path;
}
