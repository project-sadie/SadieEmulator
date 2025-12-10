using Microsoft.Extensions.Hosting;
using IServer = Sadie.API.IServer;

namespace Sadie.Console.Services;

public class ServerHostedService(IServer server) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return server.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return server.DisposeAsync().AsTask();
    }
}