namespace Sadie.Game.Rooms;

public interface IRoomDao
{
    Task<Tuple<bool, IRoom?>> TryGetRoomById(long roomId);
    Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description);
    Task<int> SaveRoomAsync(IRoom room);
    Task<List<IRoom>> GetByOwnerIdAsync(int ownerId, int limit, ICollection<long> excludeIds);
    Task<int> GetLayoutIdFromNameAsync(string name);
}