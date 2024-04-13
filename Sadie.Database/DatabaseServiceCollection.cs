using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Constants;

namespace Sadie.Database;

public static class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Default connection string is missing");
        }

        serviceCollection.AddDbContext<SadieContext>(options =>
        {
            options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion, mySqlOptions =>
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                ))
                .UseSnakeCaseNamingConvention();
        });

        serviceCollection.AddSingleton<ServerPlayerConstants>(provider =>
            provider.GetRequiredService<SadieContext>().ServerPlayerConstants.First());

        serviceCollection.AddSingleton<ServerRoomConstants>(provider =>
            provider.GetRequiredService<SadieContext>().ServerRoomConstants.First());
    }
}