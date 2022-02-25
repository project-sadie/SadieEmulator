namespace Sadie.Game.Rooms;

public interface IRoomRepository
{
    Tuple<bool, Room?> TryGetRoomById(long id);
    Task<Tuple<bool, Room?>> TryLoadRoomByIdAsync(long id);
    Task RunPeriodicCheckAsync();
}