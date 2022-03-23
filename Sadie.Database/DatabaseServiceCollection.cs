using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Sadie.Database;

public class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddTransient<IDatabaseConnection, DatabaseConnection>();
        serviceCollection.AddSingleton<DbConnectionStringBuilder>(new MySqlConnectionStringBuilder(config.GetConnectionString("Default")));
        serviceCollection.AddSingleton<IDatabaseProvider, DatabaseProvider>();
    }
}