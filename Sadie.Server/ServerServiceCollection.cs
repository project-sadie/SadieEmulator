using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database;
using Sadie.Database.Mappers;
using Sadie.Database.Models.Server;
using Sadie.Game.Catalog;
using Sadie.Game.Navigator;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Encryption;
using Sadie.Networking.Events;
using Sadie.Options;
using Sadie.Shared;
using SadieEmulator.Tasks;

namespace SadieEmulator;

public static class ServerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblyOf<IServerTask>()
            .AddClasses(classes => classes.AssignableTo<IServerTask>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        serviceCollection.AddSingleton<IServer, Server>();
        serviceCollection.AddSingleton<IServerTaskWorker, ServerTaskWorker>();
        serviceCollection.AddSingleton<ServerTaskWorker>();

        OptionsServiceCollection.AddServices(serviceCollection, config);
        DatabaseServiceCollection.AddServices(serviceCollection, config);

        NetworkingEncryptionCollection.AddServices(serviceCollection);

        serviceCollection.AddSingleton<ServerSettings>(p =>
            p.GetRequiredService<SadieContext>().ServerSettings.First());

        serviceCollection.AddSingleton<List<ServerPeriodicCurrencyReward>>(p =>
            p.GetRequiredService<SadieContext>().ServerPeriodicCurrencyRewards.ToList());

        MapperServiceCollection.AddServices(serviceCollection);
        PlayerServiceCollection.AddServices(serviceCollection);
        RoomServiceCollection.AddServices(serviceCollection);
        NetworkServiceCollection.AddServices(serviceCollection);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        NavigatorServiceCollection.AddServices(serviceCollection);
        CatalogServiceProvider.AddServices(serviceCollection);
    }
}