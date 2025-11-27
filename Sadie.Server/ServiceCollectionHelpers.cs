using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Interfaces.Game.Rooms.Chat.Commands;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Furniture.Processors;
using Sadie.API.Interfaces.Plugins;
using Serilog;

namespace SadieEmulator;

public static class ServiceCollectionHelpers
{
    extension(IServiceCollection serviceCollection)
    {
        public void RegisterRoomChatCommands(Assembly[]  assemblies)
        {
            serviceCollection.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IRoomChatCommand>())
                .As<IRoomChatCommand>()
                .WithSingletonLifetime());
        }

        public void RegisterFurnitureInteractors(Assembly[]  assemblies)
        {
            serviceCollection.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<AbstractRoomFurnitureItemInteractor>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        }

        public void RegisterRoomFurnitureProcessors(Assembly[]  assemblies)
        {
            serviceCollection.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IRoomFurnitureItemProcessor>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        }

        public void RegisterPluginServices(Assembly[]  assemblies)
        {
            var pluginServiceCollections =
                assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t =>
                        typeof(IPluginServiceCollection).IsAssignableFrom(t) &&
                        t is { IsClass: true, IsAbstract: false })
                    .Select(t => Activator.CreateInstance(t) as IPluginServiceCollection)
                    .Where(x => x is not null)!
                    .ToList();

            foreach (var pluginServiceCollection in pluginServiceCollections)
            {
                pluginServiceCollection?.Register(serviceCollection);
            }
        }
    }

    public static void LoadPlugins(IConfiguration config)
    {
        var pluginFolder = config.GetValue<string>("PluginDirectory");

        if (string.IsNullOrEmpty(pluginFolder) || !Directory.Exists(pluginFolder))
        {
            Log.Warning($"Plugin folder not found: {pluginFolder}");
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