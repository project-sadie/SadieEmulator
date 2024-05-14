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
            provider.GetRequiredService<SadieContext>()
                .ServerPlayerConstants
                .OrderByDescending(x => x.CreatedAt)
                .First()
            );

        serviceCollection.AddSingleton<ServerRoomConstants>(provider =>
            provider.GetRequiredService<SadieContext>()
                .ServerRoomConstants
                .OrderByDescending(x => x.CreatedAt)
                .First()
            );
    }
}