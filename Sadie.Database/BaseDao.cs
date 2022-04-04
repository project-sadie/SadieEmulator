using Sadie.Shared.Extensions;

namespace Sadie.Database;

public class BaseDao
{
    private readonly IDatabaseProvider _databaseProvider;

    protected BaseDao(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
    }

    protected async Task<DatabaseReader> GetReaderAsync(string commandText, Dictionary<string, object> parameters)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);
        dbConnection.AddParameters(parameters);

        var reader = await dbConnection.ExecuteReaderAsync();
        var batch = reader.ToListOfDictionaries();
        
        return new DatabaseReader(
            new Queue<DatabaseRecord>(batch.Select(x => new DatabaseRecord(x)))
        );
    }

    protected async Task<int> QueryAsync(string commandText, Dictionary<string, object> parameters)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);
        dbConnection.AddParameters(parameters);

        return await dbConnection.ExecuteNonQueryAsync();
    }

    protected async Task<int> CountAsync(string commandText, Dictionary<string, object> parameters)
    {
        using var dbConnection = _databaseProvider.GetConnection();
        dbConnection.SetQuery(commandText);
        dbConnection.AddParameters(parameters);

        return dbConnection.ExecuteScalar<int>();
    }
}