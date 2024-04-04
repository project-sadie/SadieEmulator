namespace Sadie.Database.LegacyAdoNet;

public class DatabaseReader(Queue<DatabaseRecord> records)
{
    public (bool, DatabaseRecord? record) Read()
    {
        return (records.TryDequeue(out var record), record);
    }
}