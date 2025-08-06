using Sadie.API.Plugins;

namespace SadieEmulator.Plugins;

public static class PluginService
{
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