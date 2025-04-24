using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Options.Options;

namespace Sadie.Options;

public static class OptionsServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddOptions();
        serviceCollection.Configure<PlayerOptions>(options => config.GetSection("PlayerOptions").Bind(options));
    }
}
