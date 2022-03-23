namespace Sadie.Game.Rooms;

public interface IRoomRepository : IAsyncDisposable
{
    Tuple<bool, IRoom?> TryGetRoomById(long id);
    Task<Tuple<bool, IRoom?>> TryLoadRoomByIdAsync(long id);
    Task RunPeriodicCheckAsync();
    List<IRoom> GetAll();
    int Count();
}