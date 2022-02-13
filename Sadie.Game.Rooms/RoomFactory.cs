using Sadie.Database;
using Sadie.Shared;

namespace Sadie.Game.Rooms;

public class RoomFactory
{
    private static RoomLayout CreateModelFromRecord(DatabaseRecord record)
    {
        var doorPoint = new HPoint(record.Get<int>("door_x"),
            record.Get<int>("door_y"),
            record.Get<int>("door_z"));
        
        return new RoomLayout(
            record.Get<long>("layout_id"), 
            record.Get<string>("layout_name"), 
            record.Get<string>("heightmap"), 
            doorPoint);
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