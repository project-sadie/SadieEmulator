using Sadie.Database;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryFactory
{
    public RoomCategory CreateFromRecord(DatabaseRecord record)
    {
        return new RoomCategory(
            record.Get<int>("id"), 
            record.Get<string>("caption"), 
            record.Get<int>("is_visible") == 1
        );
    }
}