namespace Sadie.Game.Rooms;

public interface IRoomRepository : IAsyncDisposable
{
    Tuple<bool, RoomLogic?> TryGetRoomById(long id);
    Task<Tuple<bool, RoomLogic?>> TryLoadRoomByIdAsync(long id);
    List<RoomLogic> GetPopularRooms(int amount);
    Task<List<RoomLogic>> GetByOwnerIdAsync(int ownerId, int amount);
    int Count { get; }
    IEnumerable<RoomLogic> GetAllRooms();
    Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description);
    Task<int> GetLayoutIdFromNameAsync(string name);
    Task SaveRoomAsync(RoomLogic room);
}