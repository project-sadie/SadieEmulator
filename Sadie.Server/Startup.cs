using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Client;
using Sadie.Networking.Events;
using Sadie.Shared;
using SadieEmulator.Tasks;
using SadieEmulator.Tasks.Game.Rooms;
using SadieEmulator.Tasks.Networking;
using SadieEmulator.Tasks.Other;
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
        var config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var sadieConstants = new SadieConstants();
        
        config.GetSection("Constants").Bind(sadieConstants);

        serviceCollection.AddSingleton(sadieConstants);
        serviceCollection.AddSingleton<IServer, Server>();
        
        serviceCollection.AddSingleton<IServerTaskWorker, ServerTaskWorker>(provider => new ServerTaskWorker(
            provider.GetRequiredService<ILogger<ServerTaskWorker>>(), 
            new List<IServerTask>
            {
                new ProcessRoomsTask(provider.GetRequiredService<IRoomRepository>()),
                new DisconnectIdleClientsTask(provider.GetRequiredService<INetworkClientRepository>()),
                new UpdateStatusTask(provider.GetRequiredService<IPlayerRepository>(), provider.GetRequiredService<IRoomRepository>()),
            }));

        DatabaseServiceCollection.AddServices(serviceCollection, config);
        PlayerServiceCollection.AddServices(serviceCollection);
        RoomServiceCollection.AddServices(serviceCollection);
        NetworkServiceCollection.AddServices(serviceCollection, config);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        
        serviceCollection.AddSingleton<ServerTaskWorker>();
    }
}