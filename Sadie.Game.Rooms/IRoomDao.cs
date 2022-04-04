namespace Sadie.Game.Rooms;

public interface IRoomDao
{
    Task<Tuple<bool, IRoom?>> TryGetRoomById(long roomId);
}