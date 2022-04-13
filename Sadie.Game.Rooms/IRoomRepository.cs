namespace Sadie.Game.Rooms;

public interface IRoomRepository : IAsyncDisposable
{
    Tuple<bool, IRoom?> TryGetRoomById(long id);
    Task<Tuple<bool, IRoom?>> TryLoadRoomByIdAsync(long id);
    List<IRoom> GetPopularRooms(int amount);
    int Count { get; }
    ICollection<IRoom> GetAllRooms();
}