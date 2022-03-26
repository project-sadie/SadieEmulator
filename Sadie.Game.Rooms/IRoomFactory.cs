using Sadie.Database;

namespace Sadie.Game.Rooms;

public interface IRoomFactory
{
    RoomLayout CreateLayoutFromRecord(DatabaseRecord record);
    Room CreateFromRecord(DatabaseRecord record);
}