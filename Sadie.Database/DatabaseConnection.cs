using System.Data;
using System.Globalization;

namespace Sadie.Database
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly IDbConnection _connection;
        private readonly IDbCommand _command;

        public DatabaseConnection(IDbConnection connection, IDbCommand command)
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

        public Task<int> ExecuteNonQueryAsync()
        {
            return Task.FromResult(_command.ExecuteNonQuery());
        }

        public Task<IDataReader> ExecuteReaderAsync()
        {
            return Task.FromResult(_command.ExecuteReader());
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
        
        public int GetLastId()
        {
            SetQuery("SELECT LAST_INSERT_ID();");
            return ExecuteScalar<int>();
        }
        
        public void Dispose()
        {
            _connection.Close();
            _command.Dispose();
        }
    }
}