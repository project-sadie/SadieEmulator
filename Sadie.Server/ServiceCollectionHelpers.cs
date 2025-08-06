using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Furniture.Processors;
using Sadie.API.Plugins;

namespace SadieEmulator;

public static class ServiceCollectionHelpers
{
    public static IServiceCollection RegisterRoomChatCommands(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IRoomChatCommand>())
            .As<IRoomChatCommand>()
            .WithSingletonLifetime());

        return serviceCollection;
    }
    
    public static IServiceCollection RegisterFurnitureInteractors(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<AbstractRoomFurnitureItemInteractor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return serviceCollection;
    }
    
    public static IServiceCollection RegisterRoomFurnitureProcessors(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        serviceCollection.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IRoomFurnitureItemProcessor>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return serviceCollection;
    }

    public static IServiceCollection RegisterPluginServices(this IServiceCollection serviceCollection, Assembly[]  assemblies)
    {
        var pluginTypes = assemblies
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(IPluginServiceConfigurator).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .ToList();

        foreach (var plugin in pluginTypes.Select(pluginType => (IPluginServiceConfigurator) Activator.CreateInstance(pluginType)!))
        {
            plugin.RegisterServicesAsync(serviceCollection);
        }

        return serviceCollection;
    }
}