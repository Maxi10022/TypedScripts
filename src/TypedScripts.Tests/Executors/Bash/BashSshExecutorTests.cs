using TypedScripts.Executors;
using TypedScripts.Tests.Fixtures;

namespace TypedScripts.Tests.Executors.Bash;

[Collection(nameof(IntegrationCollection))]
public class BashSshExecutorTests(IntegrationFixture fixture) : BashExecutorTestBase(fixture)
{
    protected override IExecutor CreateExecutor()
    {
        Client.Connect();
        return new BashSshExecutor(Client);
    }
}