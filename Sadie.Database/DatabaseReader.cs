namespace Sadie.Database;

public class DatabaseReader
{
    private readonly Queue<DatabaseRecord> _records;

    public DatabaseReader(Queue<DatabaseRecord> records)
    {
        _records = records;
    }

    public (bool, DatabaseRecord? record) Read()
    {
        return (_records.TryDequeue(out var record), record);
    }
}