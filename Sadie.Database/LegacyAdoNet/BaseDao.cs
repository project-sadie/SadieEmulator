using Sadie.Shared.Extensions;

namespace Sadie.Database.LegacyAdoNet;

public class BaseDao
{
    private readonly IDatabaseProvider _databaseProvider;

    protected BaseDao(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
    }

    protected async Task<DatabaseReader> GetReaderAsync(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        AddOptionalParameters(dbConnection, parameters);

        var reader = await dbConnection.ExecuteReaderAsync();
        var batch = reader.ToListOfDictionaries();
        
        return new DatabaseReader(
            new Queue<DatabaseRecord>(batch.Select(x => new DatabaseRecord(x)))
        );
    }

    protected async Task<int> QueryAsync(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        AddOptionalParameters(dbConnection, parameters);

        return await dbConnection.ExecuteNonQueryAsync();
    }

    protected async Task<bool> Exists(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery($"SELECT CASE WHEN EXISTS({commandText.TrimEnd([';'])}) THEN 1 ELSE 0 END");

        AddOptionalParameters(dbConnection, parameters);

        return await dbConnection.ExecuteScalarAsync<int>() == 1;
    }

    protected async Task<int> QueryScalarAsync(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        AddOptionalParameters(dbConnection, parameters);

        try
        {
            return await dbConnection.ExecuteScalarAsync<int>();
        }
        catch (InvalidCastException)
        {
            return -1;
        }
    }

    private static void AddOptionalParameters(IDatabaseConnection connection, Dictionary<string, object> parameters)
    {
        if (parameters is {Count: > 0})
        {
            connection.AddParameters(parameters);
        }
    }
}