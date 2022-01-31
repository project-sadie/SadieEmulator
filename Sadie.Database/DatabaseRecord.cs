namespace Sadie.Database;

public class DatabaseRecord
{
    private readonly Dictionary<string, object> _data;

    public DatabaseRecord(Dictionary<string, object> data)
    {
        _data = data;
    }

    public T Get<T>(string column)
    {
        return (T) Convert.ChangeType(_data[column], typeof(T));
    }
}