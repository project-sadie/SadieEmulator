using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Sadie.Database.LegacyAdoNet;

public class DatabaseProvider(MySqlConnectionStringBuilder connectionString, IServiceProvider serviceProvider)
    : IDatabaseProvider
{
    private readonly string _connectionString = connectionString.ToString();

    public IDatabaseConnection GetConnection()
    {
        var connection = new MySqlConnection(_connectionString);

        return ActivatorUtilities.CreateInstance<DatabaseConnection>(
            serviceProvider, 
            connection, 
            connection.CreateCommand());
    }

    public bool TestConnection()
    {
        try
        {
            using (GetConnection()) { }
            return true;
        }
        catch (MySqlException)
        {
            return false;
        }
    }
}