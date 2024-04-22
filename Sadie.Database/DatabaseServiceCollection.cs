using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Constants;

namespace Sadie.Database;

public static class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddDbContext<SadieContext>(ServiceLifetime.Transient);

        serviceCollection.AddSingleton<ServerPlayerConstants>(provider =>
            provider.GetRequiredService<SadieContext>().ServerPlayerConstants.First());

        serviceCollection.AddSingleton<ServerRoomConstants>(provider =>
            provider.GetRequiredService<SadieContext>().ServerRoomConstants.First());
    }
}