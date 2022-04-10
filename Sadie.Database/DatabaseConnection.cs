using System.Data;
using System.Globalization;
using MySqlConnector;

namespace Sadie.Database;

public class DatabaseConnection : IDatabaseConnection
{
    private readonly MySqlConnection _connection;
    private readonly MySqlCommand _command;

    public DatabaseConnection(MySqlConnection connection, MySqlCommand command)
    {
        _connection = connection;
        _command = command;

        _connection.Open();
    }

    public void SetQuery(string commandText)
    {
        _command.Parameters.Clear();
        _command.CommandText = commandText;
    }

    public int ExecuteNonQuery()
    {
        return _command.ExecuteNonQuery();
    }

    public async Task<int> ExecuteNonQueryAsync()
    {
        return await _command.ExecuteNonQueryAsync();
    }

    public async Task<MySqlDataReader> ExecuteReaderAsync()
    {
        return await _command.ExecuteReaderAsync();
    }

    public void AddParameter(string name, object value)
    {
        var parameter = _command.CreateParameter();
            
        parameter.ParameterName = $"@{name}";
        parameter.Value = value;

        _command.Parameters.Add(parameter);
    }

    public void AddParameters(Dictionary<string, object> parameters)
    {
        foreach (var (key, value) in parameters)
        {
            AddParameter(key, value);
        }
    }
        
    public T ExecuteScalar<T>()
    {
        return (T) Convert.ChangeType(_command.ExecuteScalar(), typeof(T), CultureInfo.InvariantCulture)!;
    }
        
    public async Task<T> ExecuteScalarAsync<T>()
    {
        return (T) Convert.ChangeType(await _command.ExecuteScalarAsync(), typeof(T), CultureInfo.InvariantCulture)!;
    }
        
    public async Task<int> GetLastIdAsync()
    {
        SetQuery("SELECT LAST_INSERT_ID();");
        return await ExecuteScalarAsync<int>();
    }
        
    public void Dispose()
    {
        _connection.Close();
        _command.Dispose();
    }
}