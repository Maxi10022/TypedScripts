using Renci.SshNet;

namespace TypedScripts.Tests.Fixtures;

public abstract class IntegrationTestBase(IntegrationFixture fixture) : IAsyncLifetime
{
    protected SshClient Client { get; private set; } = null!;
    
    public Task InitializeAsync()
    {
        var connectionInfo = fixture.GetRemoteConnectionInfo();
        Client = new SshClient(connectionInfo);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }
}