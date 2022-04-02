using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Categories;
using Sadie.Networking;
using SadieEmulator.Tasks;

namespace SadieEmulator;

public class Server : IServer
{
    public static readonly Version Version = new(0, 5);

    private readonly ILogger<Server> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Server(ILogger<Server> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task RunAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        WriteHeaderToConsole();
            
        _logger.LogTrace("Booting up...");

        var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();

        if (!dbProvider.TestConnection())
        {
            throw new Exception("Failed to connect to the database.");
        }

        _logger.LogTrace("Database connection is working!");
        
        var roomCategoryRepo = _serviceProvider.GetRequiredService<IRoomCategoryRepository>();

        await roomCategoryRepo.LoadInitialDataAsync();
        _logger.LogTrace("Loaded room categories");
        
        var navigatorTabRepo = _serviceProvider.GetRequiredService<IRoomCategoryRepository>();

        await navigatorTabRepo.LoadInitialDataAsync();
        _logger.LogTrace("Loaded navigator tabs");

        var taskWorker = _serviceProvider.GetRequiredService<IServerTaskWorker>();
        taskWorker.Start();
        
        _logger.LogTrace("Loaded game services");
        
        stopwatch.Stop();
        
        _logger.LogInformation($"Server booted up in {Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms");

        var networkListener = _serviceProvider.GetRequiredService<INetworkListener>();

        networkListener.Start();
        await networkListener.ListenAsync();
    }
    
    private static void WriteHeaderToConsole()
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
        
        Console.WriteLine($"         You are running version {Version}");
        Console.WriteLine(@"");
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogWarning("Service is about to shut down...");
        
        var roomRepository = _serviceProvider.GetRequiredService<IRoomRepository>();
        var playerRepository = _serviceProvider.GetRequiredService<IPlayerRepository>();
        
        _logger.LogInformation("Disposing rooms...");
        await roomRepository.DisposeAsync();
        
        _logger.LogInformation("Disposing players...");
        await playerRepository.DisposeAsync();
    }
}