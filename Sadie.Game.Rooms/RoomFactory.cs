using Sadie.Database;

namespace Sadie.Game.Rooms;

public class RoomFactory
{
    public static RoomModel CreateModelFromRecord(DatabaseRecord @record)
    {
        return new RoomModel(record.Get<long>("layout_id"), record.Get<string>("layout_name"), record.Get<string>("heightmap"));
    }
    
    public static RoomEntity CreateFromRecord(DatabaseRecord record)
    {
        var model = CreateModelFromRecord(record);
        
        return new RoomEntity(
            record.Get<long>("id"),
            record.Get<string>("name"),
            model
        );
    }
}