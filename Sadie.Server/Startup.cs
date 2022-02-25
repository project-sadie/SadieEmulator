using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sadie.Database;
using Sadie.Game;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Events;
using Serilog;

namespace SadieEmulator;

public static class Startup
{
    public static IHost CreateDefaultHostBuilder()
    {
        return Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .UseSerilog((hostContext, _, logger) => 
                    logger.ReadFrom.Configuration(hostContext.Configuration))
                .Build();
    }

    private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IServer, Server>();
        
        var config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            
        DatabaseServiceCollection.AddServices(serviceCollection, config);
        PlayerServiceCollection.AddServices(serviceCollection);
        RoomServiceCollection.AddServices(serviceCollection);
        NetworkServiceCollection.AddServices(serviceCollection, config);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        GameService.AddServices(serviceCollection);
    }
}