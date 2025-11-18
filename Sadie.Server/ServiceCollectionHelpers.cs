using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Interfaces.Game.Rooms.Chat.Commands;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Furniture.Processors;

namespace SadieEmulator;

public static class ServiceCollectionHelpers
{
    public static void RegisterRoomChatCommands(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IRoomChatCommand>())
            .As<IRoomChatCommand>()
            .WithSingletonLifetime());
    }
    
    public static void RegisterFurnitureInteractors(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<AbstractRoomFurnitureItemInteractor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
    
    public static void RegisterRoomFurnitureProcessors(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IRoomFurnitureItemProcessor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public static void LoadPlugins(IConfiguration config)
    {
        var pluginFolder = config.GetValue<string>("PluginDirectory");

        if (string.IsNullOrEmpty(pluginFolder) || !Directory.Exists(pluginFolder))
        {
            return;
        }
        
        foreach (var plugin in Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories))
        {
            var assembly = Assembly.LoadFile(plugin);
            var version = assembly.GetName().Version;
            
            Console.WriteLine($"Loaded plugin: {Path.GetFileNameWithoutExtension(plugin)} {version}");
        }
    }
}