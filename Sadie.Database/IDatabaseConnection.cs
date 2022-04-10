using MySqlConnector;

namespace Sadie.Database;

public interface IDatabaseConnection : IDisposable
{
    void SetQuery(string commandText);
    int ExecuteNonQuery();
    Task<int> ExecuteNonQueryAsync();
    Task<MySqlDataReader> ExecuteReaderAsync();
    void AddParameter(string name, object value);
    void AddParameters(Dictionary<string, object> parameters);
    T ExecuteScalar<T>();
    Task<T> ExecuteScalarAsync<T>();
    Task<int> GetLastIdAsync();
}