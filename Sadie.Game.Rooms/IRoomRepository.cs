namespace Sadie.Game.Rooms;

public interface IRoomRepository : IAsyncDisposable
{
    Tuple<bool, IRoom?> TryGetRoomById(long id);
    Task<Tuple<bool, IRoom?>> TryLoadRoomByIdAsync(long id);
    List<IRoom> GetPopularRooms(int amount);
    Task<List<IRoom>> GetByOwnerIdAsync(int ownerId, int amount);
    int Count { get; }
    IEnumerable<IRoom> GetAllRooms();
    Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description);
    Task<int> CreateRoomSettingsAsync(int roomId);
    Task<int> CreatePaintSettingsAsync(int roomId);
    Task<int> GetLayoutIdFromNameAsync(string name);
    Task SaveRoomAsync(IRoom room);
}