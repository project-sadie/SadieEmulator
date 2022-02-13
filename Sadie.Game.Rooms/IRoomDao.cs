namespace Sadie.Game.Rooms;

public interface IRoomDao
{
    Task<Tuple<bool, Room?>> TryGetRoomById(long roomId);
}