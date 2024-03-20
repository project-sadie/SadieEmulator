using Sadie.Game.Rooms.Chat;

namespace Sadie.Game.Rooms;

public interface IRoomDao
{
    Task<Tuple<bool, IRoom?>> TryGetRoomById(long roomId);
    Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description);
    Task<int> CreateRoomSettings(int roomId);
    Task<int> GetLayoutIdFromNameAsync(string name);
    Task<int> CreateChatMessages(List<RoomChatMessage> messages);
    Task<int> SaveRoomAsync(IRoom room);
    Task<List<IRoom>> GetByOwnerIdAsync(int ownerId, int limit, ICollection<long> excludeIds);
    Task InsertRightsAsync(int roomId, int playerId);
    Task DeleteRightsAsync(int roomId, int playerId);
}