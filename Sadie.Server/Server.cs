using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Rooms.Categories;
using Sadie.Networking;

namespace SadieEmulator;

public class Server : IServer
{
    private static readonly Version Version = new(0, 2);

    private readonly ILogger<Server> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Server(ILogger<Server> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task RunAsync()
    {
        WriteHeaderToConsole();
            
        _logger.LogInformation("Booting up...");

        var dbProvider = _serviceProvider.GetRequiredService<IDatabaseProvider>();

        if (!dbProvider.TestConnection())
        {
            throw new Exception("Failed to connect to the database.");
        }

        _logger.LogInformation("Database connection is working!");
        
        var roomCategoryRepo = _serviceProvider.GetRequiredService<IRoomCategoryRepository>();

        await roomCategoryRepo.LoadInitialDataAsync();
        _logger.LogInformation("Loaded room categories");
        
        _logger.LogDebug("Server has booted up.");
        
        var networkListener = _serviceProvider.GetRequiredService<INetworkListener>();

        networkListener.Start();
        await networkListener.ListenAsync();
    }
    
    private static void WriteHeaderToConsole()
    {
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
        Console.WriteLine($"         You are running version {Version}");
        Console.WriteLine(@"");
    }

    public void Dispose()
    {
        
    }
}