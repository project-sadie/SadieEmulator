namespace Sadie.Game.Rooms;

public interface IRoomDao
{
    Task<Tuple<bool, RoomEntity?>> TryGetRoomById(long roomId);
}