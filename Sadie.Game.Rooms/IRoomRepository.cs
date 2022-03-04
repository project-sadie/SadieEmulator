namespace Sadie.Game.Rooms;

public interface IRoomRepository : IDisposable
{
    Tuple<bool, IRoom?> TryGetRoomById(long id);
    Task<Tuple<bool, IRoom?>> TryLoadRoomByIdAsync(long id);
    Task RunPeriodicCheckAsync();
}