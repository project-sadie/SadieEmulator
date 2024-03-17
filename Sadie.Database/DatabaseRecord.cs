namespace Sadie.Database;

public class DatabaseRecord(Dictionary<string, object> data)
{
    public T Get<T>(string column)
    {
        return (T) Convert.ChangeType(data[column], typeof(T));
    }
}