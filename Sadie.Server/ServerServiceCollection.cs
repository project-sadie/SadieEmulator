using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API;
using Sadie.Database.Mappers;
using Sadie.Db;
using Sadie.Db.Models.Server;
using Sadie.Game.Navigator;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Migrations;
using Sadie.Networking;
using Sadie.Networking.Encryption;
using Sadie.Networking.Events;
using SadieEmulator.Tasks;

namespace SadieEmulator;

public static class ServerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        serviceCollection.AddOptions();
        serviceCollection.AddSingleton<IServer, Server>();
        serviceCollection.AddSingleton<IServerTaskWorker, ServerTaskWorker>();
        serviceCollection.AddSingleton<ServerTaskWorker>();

        serviceCollection.AddDbContextFactory<SadieMigrationsContext>();
        DatabaseServiceCollection.AddServices(serviceCollection, config);

        serviceCollection.AddSingleton<ServerSettings>(p => new ServerSettings());
        
        serviceCollection.AddSingleton<List<ServerPeriodicCurrencyReward>>(p => []);

        MapperServiceCollection.AddServices(serviceCollection);
        PlayerServiceCollection.AddServices(serviceCollection, config);
        RoomServiceCollection.AddServices(serviceCollection);
        NetworkServiceCollection.AddServices(serviceCollection, config);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        NavigatorServiceCollection.AddServices(serviceCollection);
        EncryptionServiceProvider.AddServices(serviceCollection, config);
        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IServerTask>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}