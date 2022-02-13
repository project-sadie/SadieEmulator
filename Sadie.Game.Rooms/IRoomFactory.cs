using Sadie.Database;

namespace Sadie.Game.Rooms;

public interface IRoomFactory
{
    RoomLayout CreateModelFromRecord(DatabaseRecord record);
    Room CreateFromRecord(DatabaseRecord record);
}