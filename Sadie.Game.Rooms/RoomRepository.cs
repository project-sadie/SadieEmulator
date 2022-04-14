using System.Collections.Concurrent;
using Sadie.Game.Rooms.Chat;

namespace Sadie.Game.Rooms;

public class RoomRepository : IRoomRepository
{
    private readonly IRoomDao _dao;
    private readonly ConcurrentDictionary<long, IRoom> _rooms;

    public RoomRepository(IRoomDao dao)
    {
        _dao = dao;
        _rooms = new ConcurrentDictionary<long, IRoom>();
    }
    
    public Tuple<bool, IRoom?> TryGetRoomById(long id)
    {
        return new Tuple<bool, IRoom?>(_rooms.TryGetValue(id, out var room), room);
    }
    
    public async Task<Tuple<bool, IRoom?>> TryLoadRoomByIdAsync(long id)
    {
        var (memoryResult, memoryValue) = TryGetRoomById(id);

        if (memoryResult)
        {
            return new Tuple<bool, IRoom?>(true, memoryValue);
        }
        
        var (result, room) = await _dao.TryGetRoomById(id);

        if (result && room != null)
        {
            _rooms[room.Id] = room;
        }

        return TryGetRoomById(id);
    }

    public List<IRoom> GetPopularRooms(int amount)
    {
        return _rooms.Values.
            Where(x => x.UserRepository.Count > 0).
            OrderByDescending(x => x.UserRepository.Count).
            Take(amount).
            ToList();
    }

    public int Count => _rooms.Count;
    public IEnumerable<IRoom> GetAllRooms() => _rooms.Values;

    public async Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description)
    {
        return await _dao.CreateRoomAsync(name, layoutId, ownerId, maxUsers, description);
    }

    public async Task<int> CreateRoomSettings(int roomId)
    {
        return await _dao.CreateRoomSettings(roomId);
    }

    public async Task<int> GetLayoutIdFromNameAsync(string name)
    {
        return await _dao.GetLayoutIdFromNameAsync(name);
    }

    public async Task<int> CreateChatMessages(List<RoomChatMessage> messages)
    {
        return await _dao.CreateChatMessages(messages);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var room in _rooms.Values)
        {
            await room.DisposeAsync();
        }
        
        _rooms.Clear();
    }
}