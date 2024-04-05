using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Sadie.Database.LegacyAdoNet;

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
        
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
        
        serviceCollection.AddSingleton(connectionStringBuilder);
        serviceCollection.AddTransient<IDatabaseConnection, DatabaseConnection>();
        serviceCollection.AddSingleton<IDatabaseProvider, DatabaseProvider>();

        serviceCollection.AddDbContext<SadieContext>(options => options
            .UseMySql(connectionString, MariaDbServerVersion.LatestSupportedServerVersion) 
            .UseSnakeCaseNamingConvention());
    }
}