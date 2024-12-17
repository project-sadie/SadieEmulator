using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API;
using Sadie.Database;
using Sadie.Database.Mappers;
using Sadie.Database.Models.Server;
using Sadie.Game.Catalog;
using Sadie.Game.Navigator;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking;
using Sadie.Networking.Events;
using Sadie.Options;
using SadieEmulator.Tasks;

namespace SadieEmulator;

public static class ServerServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var pluginFolder = config.GetValue<string>("PluginDirectory");

        if (!string.IsNullOrEmpty(pluginFolder) && Directory.Exists(pluginFolder))
        {
            foreach (var plugin in Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories))
            {
                Assembly.LoadFile(plugin);
            }
        }

        serviceCollection.AddSingleton<IServer, Server>();
        serviceCollection.AddSingleton<IServerTaskWorker, ServerTaskWorker>();
        serviceCollection.AddSingleton<ServerTaskWorker>();

        OptionsServiceCollection.AddServices(serviceCollection, config);
        DatabaseServiceCollection.AddServices(serviceCollection, config);

        serviceCollection.AddSingleton<ServerSettings>(p => 
            p.GetRequiredService<SadieContext>()
                .ServerSettings
                .First());
        
        serviceCollection.AddSingleton<List<ServerPeriodicCurrencyReward>>(p => 
            p.GetRequiredService<SadieContext>().ServerPeriodicCurrencyRewards.ToList());

        MapperServiceCollection.AddServices(serviceCollection);
        PlayerServiceCollection.AddServices(serviceCollection);
        RoomServiceCollection.AddServices(serviceCollection, config);
        NetworkServiceCollection.AddServices(serviceCollection);
        NetworkPacketServiceCollection.AddServices(serviceCollection);
        NavigatorServiceCollection.AddServices(serviceCollection);
        CatalogServiceProvider.AddServices(serviceCollection);
        
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IServerTask>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}