using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database;
using Sadie.Database.Mappers;
using Sadie.Database.Models;
using Sadie.Game.Catalog;
using Sadie.Game.Furniture;
using Sadie.Game.Navigator;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Events;
using Sadie.Options;
using Sadie.Shared;
using SadieEmulator.Tasks;
using SadieEmulator.Tasks.Game.Rooms;
using SadieEmulator.Tasks.Networking;
using SadieEmulator.Tasks.Other;

namespace SadieEmulator;

public static class ServerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<IServerTask, ProcessRoomsTask>();
        serviceCollection.AddSingleton<IServerTask, DisconnectIdleClientsTask>();
        serviceCollection.AddSingleton<IServerTask, UpdateConsoleTitleTask>();

        serviceCollection.AddSingleton<IServer, Server>();
        serviceCollection.AddSingleton<IServerTaskWorker, ServerTaskWorker>();
        serviceCollection.AddSingleton<ServerTaskWorker>();

        OptionsServiceCollection.AddServices(serviceCollection, config);
        DatabaseServiceCollection.AddServices(serviceCollection, config);

        serviceCollection.AddSingleton<ServerSettings>(p => p.GetRequiredService<SadieContext>().ServerSettings.First());

        MapperServiceCollection.AddServices(serviceCollection);
        PlayerServiceCollection.AddServices(serviceCollection);
        RoomServiceCollection.AddServices(serviceCollection);
        NetworkServiceCollection.AddServices(serviceCollection);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        NavigatorServiceCollection.AddServices(serviceCollection);
        CatalogServiceProvider.AddServices(serviceCollection);
        FurnitureServiceProvider.AddServices(serviceCollection);
    }
}