using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Sadie.Database;

public class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        var connectionStringBuilder = new MySqlConnectionStringBuilder(config.GetConnectionString("Default"));
        
        serviceCollection.AddSingleton(connectionStringBuilder);
        serviceCollection.AddTransient<IDatabaseConnection, DatabaseConnection>();
        serviceCollection.AddSingleton<IDatabaseProvider, DatabaseProvider>();
    }
}