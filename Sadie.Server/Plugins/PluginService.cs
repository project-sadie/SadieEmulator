using System.Reflection;
using Microsoft.Extensions.Configuration;
using Sadie.API.Plugins;

namespace SadieEmulator.Plugins;

public static class PluginService
{
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
    
    public static async Task BootstrapPluginsAsync()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        var pluginTypes = assemblies
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(IPluginBootstrapper).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .ToList();

        foreach (var plugin in pluginTypes.Select(pluginType => (IPluginBootstrapper) Activator.CreateInstance(pluginType)!))
        {
            await plugin.BootstrapAsync();
        }
    }
}