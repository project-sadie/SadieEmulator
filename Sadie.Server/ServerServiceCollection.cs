using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API;
using Sadie.API.Interfaces.Server;
using Sadie.API.Interfaces.Server.Tasks;
using Sadie.Db;
using Sadie.Db.Models.Server;
using Sadie.Game;
using Sadie.Game.Catalog;
using Sadie.Game.Locale;
using Sadie.Game.Mappers;
using Sadie.Game.Navigator;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Encryption;
using Sadie.Networking.Events;
using Sadie.Server.Infrastructure;
using Sadie.Server.Tasks;

namespace Sadie.Server;

public static class ServerServiceCollection
{
    public static void AddServices(IServiceCollection services, IConfiguration config)
    {
        services.AddOptions();
        
        services.AddSingleton<IServer, Server>();
        services.AddSingleton<IServerMigrator, ServerMigrator>();
        services.AddSingleton<IServerDataCleaner, ServerDataCleaner>();
        services.AddSingleton<IServerTaskWorker, ServerTaskWorker>();

        MapperServiceCollection.AddServices(services);
        DatabaseServiceCollection.AddServices(services, config);
        NetworkServiceCollection.AddServices(services, config);
        NetworkPacketServiceCollection.AddServices(services);

        services.AddSingleton(new ServerSettings());
        services.AddSingleton(new List<ServerPeriodicCurrencyReward>());
        services.AddHostedService<GameWorker>();
        
        RegisterGameServices(services, config);

        ServiceCollectionHelpers.LoadPlugins(config);
        
        RegisterReflectionDiscoveredServices(services);
    }
    
    private static void RegisterGameServices(IServiceCollection services, IConfiguration config)
    {
        PlayerServiceCollection.AddServices(services, config);
        RoomServiceCollection.AddServices(services);
        NavigatorServiceCollection.AddServices(services);
        EncryptionServiceProvider.AddServices(services, config);
        LocaleServiceCollection.AddServices(services);
        CatalogServiceCollection.AddServices(services, config);
    }
    
    private static void RegisterReflectionDiscoveredServices(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.RegisterRoomChatCommands(assemblies);
        services.RegisterFurnitureInteractors(assemblies);
        services.RegisterRoomFurnitureProcessors(assemblies);
        services.RegisterPluginServices(assemblies);

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo<IServerTask>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}