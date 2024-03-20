using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Furniture;
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Categories;
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

    private async Task LoadInitialDataAsync()
    {
        var roomCategoryRepo = serviceProvider.GetRequiredService<IRoomCategoryRepository>();
        await roomCategoryRepo.LoadInitialDataAsync();
        logger.LogTrace("Loaded room categories");
        
        var navigatorTabRepo = serviceProvider.GetRequiredService<NavigatorTabRepository>();
        await navigatorTabRepo.LoadInitialDataAsync();
        logger.LogTrace("Loaded navigator tabs");

        var furnitureItemRepository = serviceProvider.GetRequiredService<FurnitureItemRepository>();
        await furnitureItemRepository.LoadInitialDataAsync();
        logger.LogTrace("Loaded furniture items");

        var catalogPagesRepo = serviceProvider.GetRequiredService<CatalogPageRepository>();
        await catalogPagesRepo.LoadInitialDataAsync();
        logger.LogTrace("Loaded catalog pages");
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
        logger.LogWarning("Service is about to shut down...");
        
        var roomRepository = serviceProvider.GetRequiredService<IRoomRepository>();
        var playerRepository = serviceProvider.GetRequiredService<IPlayerRepository>();
        
        logger.LogInformation("Disposing rooms...");
        await roomRepository.DisposeAsync();
        
        logger.LogInformation("Disposing players...");
        await playerRepository.DisposeAsync();
    }
}