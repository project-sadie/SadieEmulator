using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Sadie.Database;

public static class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");

        if (string.IsNullOrEmpty(connectionString))
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
    }
}