using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Networking;
using Sadie.Shared;
using SadieEmulator.Tasks;
using System.Diagnostics;
using Sadie.API;
using Sadie.Networking.Client;

namespace SadieEmulator;

public class Server(ILogger<Server> logger, IServiceProvider serviceProvider) : IServer
{
    private readonly CancellationTokenSource _tokenSource = new();
    
    public async Task RunAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        WriteHeaderToConsole();
        await CleanUpDataAsync();
        
        serviceProvider.GetRequiredService<IServerTaskWorker>().WorkAsync(_tokenSource.Token);

        stopwatch.Stop();

        logger.LogInformation($"Server booted up in {Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms");
        
        await StartListeningForConnectionsAsync();
    }

    private async Task StartListeningForConnectionsAsync()
    {
        var networkListener = serviceProvider.GetRequiredService<INetworkListener>();
        networkListener.Bootstrap();
        
        await networkListener.ListenAsync();
    }

    private async Task CleanUpDataAsync()
    {
        var context = serviceProvider.GetRequiredService<SadieContext>();

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

        var networkClientRepository = serviceProvider.GetRequiredService<INetworkClientRepository>();
        await networkClientRepository.DisposeAsync();
    }
}