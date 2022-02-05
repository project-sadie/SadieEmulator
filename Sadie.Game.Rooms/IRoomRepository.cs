namespace Sadie.Game.Rooms;

public interface IRoomRepository
{
    Tuple<bool, RoomEntity?> TryGetRoomById(long id);
    Task<Tuple<bool, RoomEntity?>> TryLoadRoomByIdAsync(long id);
}