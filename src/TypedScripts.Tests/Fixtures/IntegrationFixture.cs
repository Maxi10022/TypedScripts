using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Renci.SshNet;

namespace TypedScripts.Tests.Fixtures;

public class IntegrationFixture : IAsyncLifetime
{
    private IContainer? _shellContainer;

    public ConnectionInfo GetRemoteConnectionInfo()
    {
        ArgumentNullException.ThrowIfNull(_shellContainer);
        return new ConnectionInfo(
            host: _shellContainer.Hostname,
            port: _shellContainer.GetMappedPublicPort(containerPort: 22),
            username: "tester",
            authenticationMethods: new PasswordAuthenticationMethod("tester", "Passw0rd!")
        );
    }

    public async Task InitializeAsync()
    {
        // Set up and create docker image
        var image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetProjectDirectory(), "Fixtures")
            .WithDockerfile("Dockerfile")
            .Build();

        await image.CreateAsync();
        
        // Set up and start shell container 
        _shellContainer = new ContainerBuilder(image)
            .WithPortBinding(22, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();
        
        await _shellContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_shellContainer is null) return;
        await _shellContainer.DisposeAsync();
    }
}