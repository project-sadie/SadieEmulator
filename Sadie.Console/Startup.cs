using Microsoft.Extensions.Hosting;
using SadieEmulator;
using Serilog;

namespace Sadie.Console;

public static class Startup
{
    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, collection) => ServerServiceCollection.AddServices(collection, context.Configuration))
            .UseSerilog((hostContext, _, logger) => logger.ReadFrom.Configuration(hostContext.Configuration))
            .Build();
    }
}