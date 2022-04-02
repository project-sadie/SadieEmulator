using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SadieEmulator;
using Serilog;

namespace Sadie.Console;

public static class Startup
{
    public static IHost CreateDefaultHostBuilder()
    {
        return Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .UseSerilog((hostContext, _, logger) => 
                    logger.ReadFrom.Configuration(hostContext.Configuration))
                .Build();
    }

    private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
    {
        var config = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
        serviceCollection.AddSingleton<IServer, Server>();
        ServerServiceCollection.AddServices(serviceCollection, config);
    }
}