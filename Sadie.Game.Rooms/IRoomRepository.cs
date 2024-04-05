namespace Sadie.Game.Rooms;

public interface IRoomRepository : IAsyncDisposable
{
    Tuple<bool, Room?> TryGetRoomById(long id);
    Task<Tuple<bool, Room?>> TryLoadRoomByIdAsync(long id);
    List<Room> GetPopularRooms(int amount);
    Task<List<Room>> GetByOwnerIdAsync(int ownerId, int amount);
    int Count { get; }
    IEnumerable<Room> GetAllRooms();
    Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description);
    Task<int> GetLayoutIdFromNameAsync(string name);
    Task SaveRoomAsync(Room room);
}