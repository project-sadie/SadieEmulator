using Sadie.Shared.Extensions;

namespace Sadie.Database;

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

    protected async Task<int> CountAsync(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);

        AddOptionalParameters(dbConnection, parameters);

        return await dbConnection.ExecuteScalarAsync<int>();
    }

    protected async Task<bool> Exists(string commandText, Dictionary<string, object> parameters = null!)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery($"SELECT CASE WHEN EXISTS({commandText.TrimEnd(new []{';'})}) THEN 1 ELSE 0 END");

        AddOptionalParameters(dbConnection, parameters);

        return await dbConnection.ExecuteScalarAsync<int>() == 1;
    }

    private static void AddOptionalParameters(IDatabaseConnection connection, Dictionary<string, object> parameters)
    {
        if (parameters is {Count: > 0})
        {
            connection.AddParameters(parameters);
        }
    }
}