namespace Sadie.Database.LegacyAdoNet;

public class DatabaseRecord(IDictionary<string, object> data)
{
    public T Get<T>(string column)
    {
        return (T) Convert.ChangeType(data[column], typeof(T));
    }
}