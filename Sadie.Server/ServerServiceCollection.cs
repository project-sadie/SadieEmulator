using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database;
using Sadie.Game.Navigator;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Client;
using Sadie.Networking.Events;
using SadieEmulator.Tasks;
using SadieEmulator.Tasks.Game.Rooms;
using SadieEmulator.Tasks.Networking;
using SadieEmulator.Tasks.Other;

namespace SadieEmulator;

public class ServerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton(provider => new List<IServerTask>
        {
            new ProcessRoomsTask(provider.GetRequiredService<IRoomRepository>()),
            new StoreChatMessagesTask(provider.GetRequiredService<IRoomRepository>()),
            new DisconnectIdleClientsTask(provider.GetRequiredService<INetworkClientRepository>()),
            new UpdateStatusTask(provider.GetRequiredService<IPlayerRepository>(), provider.GetRequiredService<IRoomRepository>()),
        });
        
        serviceCollection.AddSingleton<IServer, Server>();
        serviceCollection.AddSingleton<IServerTaskWorker, ServerTaskWorker>();
        serviceCollection.AddSingleton<ServerTaskWorker>();
        
        DatabaseServiceCollection.AddServices(serviceCollection, config);
        PlayerServiceCollection.AddServices(serviceCollection, config);
        RoomServiceCollection.AddServices(serviceCollection, config);
        NetworkServiceCollection.AddServices(serviceCollection, config);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        NavigatorServiceCollection.AddServices(serviceCollection);
    }
}