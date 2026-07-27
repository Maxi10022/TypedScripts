using TypedScripts.Executors;
using TypedScripts.Tests.Fixtures;

namespace TypedScripts.Tests.Executors.Bash;

[Collection(nameof(IntegrationCollection))]
public class BashLocalExecutorTests(IntegrationFixture fixture) : BashExecutorTestBase(fixture)
{
    protected override IExecutor CreateExecutor() => new BashLocalExecutor();
}
