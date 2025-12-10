using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API;
using Sadie.Game.Players.Options;

namespace Sadie.Console.Services;

public class ServerHostedService(
    ILogger<ServerHostedService> logger,
    IServer server,
    IOptions<PlayerOptions> playerOptions) : IHostedService
{
    public async Task StartAsync(CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        
        if (playerOptions.Value.CanReuseSsoTokens)
        {
            logger.LogWarning("Reusable SSO tokens enabled — reduced security.");
        }

        logger.LogInformation("Boot sequence initiated...");
        await server.RunAsync(token);

        sw.Stop();
        logger.LogInformation("Boot completed in {ms} ms", sw.ElapsedMilliseconds);
    }

    public Task StopAsync(CancellationToken token)
    {
        logger.LogWarning("Server is shutting down...");
        return server.DisposeAsync().AsTask();
    }
}