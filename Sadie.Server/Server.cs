using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Catalog.FrontPage;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Furniture;
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Players;
using Sadie.Game.Players.Club;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Shared;
using SadieEmulator.Tasks;

namespace SadieEmulator;

public class Server(ILogger<Server> logger, IServiceProvider serviceProvider) : IServer
{
    public async Task RunAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        WriteHeaderToConsole();
        TestDatabaseConnection();

        await CleanUpDataAsync();
        await LoadInitialDataAsync();
        
        serviceProvider.GetRequiredService<IServerTaskWorker>().Start();

        stopwatch.Stop();
        
        logger.LogInformation($"Server booted up in {Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms");

        await StartListeningForConnectionsAsync();
    }

    private async Task StartListeningForConnectionsAsync()
    {
        var networkListener = serviceProvider.GetRequiredService<INetworkListener>();

        networkListener.Start();
        await networkListener.ListenAsync();
    }

    private void TestDatabaseConnection()
    {
        var dbProvider = serviceProvider.GetRequiredService<IDatabaseProvider>();

        if (!dbProvider.TestConnection())
        {
            throw new Exception("Failed to connect to the database.");
        }

        logger.LogTrace("Database connection is working!");
    }

    private async Task CleanUpDataAsync()
    {
        var context = serviceProvider.GetRequiredService<SadieContext>();
        
        var affected = await context
            .Players
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.SsoToken, b => null!));
        
        logger.LogWarning($"Reset sso token for {affected} players");
    }

    private async Task LoadInitialDataAsync()
    {
        await LoadInitialDataAsync(serviceProvider.GetRequiredService<NavigatorTabRepository>().LoadInitialDataAsync, "navigator tabs");
        await LoadInitialDataAsync(serviceProvider.GetRequiredService<FurnitureItemRepository>().LoadInitialDataAsync, "furniture items");
        await LoadInitialDataAsync(serviceProvider.GetRequiredService<CatalogPageRepository>().LoadInitialDataAsync, "catalog pages");
        await LoadInitialDataAsync(serviceProvider.GetRequiredService<CatalogFrontPageItemRepository>().LoadInitialDataAsync, "catalog front page items");
        await LoadInitialDataAsync(serviceProvider.GetRequiredService<PlayerClubOfferRepository>().LoadInitialDataAsync, "player club offers");
    }

    private async Task LoadInitialDataAsync(Func<Task> action, string name)
    {
        logger.LogTrace($"Loading {name}...");
        await action.Invoke();
    }
    
    private void WriteHeaderToConsole()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        
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
        logger.LogWarning("Server is about to shut down...");
        
        var roomRepository = serviceProvider.GetRequiredService<RoomRepository>();
        var playerRepository = serviceProvider.GetRequiredService<PlayerRepository>();
        
        logger.LogInformation("Disposing rooms...");
        await roomRepository.DisposeAsync();
        
        logger.LogInformation("Disposing players...");
        await playerRepository.DisposeAsync();
    }
}