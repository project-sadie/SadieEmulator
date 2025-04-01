using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Networking;
using Sadie.Shared;
using SadieEmulator.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Sadie.API;
using Sadie.Networking.Client;
using Sadie.Options.Options;
using Serilog;

namespace SadieEmulator;

public class Server(ILogger<Server> logger,
    IServerTaskWorker taskWorker,
    INetworkListener networkListener,
    IDbContextFactory<SadieContext> dbContextFactory,
    IOptions<PlayerOptions> playerOptions,
    INetworkClientRepository networkClientRepository) : IServer
{
    private readonly CancellationTokenSource _tokenSource = new();
    
    public async Task RunAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        WriteHeaderToConsole();
        await CleanUpDataAsync();

        if (playerOptions.Value.CanReuseSsoTokens)
        {
            Log.Logger.Warning($"Reusable SSO tokens activated, this results in reduced security.");
        }
        
        taskWorker.WorkAsync(_tokenSource.Token);

        stopwatch.Stop();

        logger.LogInformation($"Server booted up in {Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms");
        
        await StartListeningForConnectionsAsync();
    }

    private async Task StartListeningForConnectionsAsync()
    {
        networkListener.Bootstrap();
        await networkListener.ListenAsync();
    }

    private async Task CleanUpDataAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        await context
            .PlayerData
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsOnline, b => false));
    }

    private void WriteHeaderToConsole()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine(@"");
        Console.WriteLine(@"   $$$$$$\                  $$\ $$\           ");
        Console.WriteLine(@"  $$  __$$\                 $$ |\__|          ");
        Console.WriteLine(@"  $$ /  \__| $$$$$$\   $$$$$$$ |$$\  $$$$$$\  ");
        Console.WriteLine(@"  \$$$$$$\   \____$$\ $$  __$$ |$$ |$$  __$$\ ");
        Console.WriteLine(@"   \____$$\  $$$$$$$ |$$ /  $$ |$$ |$$$$$$$$ |");
        Console.WriteLine(@"  $$\   $$ |$$  __$$ |$$ |  $$ |$$ |$$   ____|");
        Console.WriteLine(@"  \$$$$$$  |\$$$$$$$ |\$$$$$$$ |$$ |\$$$$$$$\ ");
        Console.WriteLine(@"   \______/  \_______| \_______|\__| \_______|");
        Console.WriteLine(@"");

        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine($"         You are running version {GlobalState.Version}");
        Console.WriteLine(@"");

        logger.LogTrace("Booting up...");
    }

    public async ValueTask DisposeAsync()
    {
        await _tokenSource.CancelAsync();
        
        logger.LogWarning("Server is about to shut down...");

        await networkClientRepository.DisposeAsync();
    }
}