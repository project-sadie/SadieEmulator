using System.Collections.Concurrent;

namespace Sadie.Game.Rooms;

public class RoomRepository(IRoomDao dao) : IRoomRepository
{
    private readonly ConcurrentDictionary<long, IRoom> _rooms = new();

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
        
        var (result, room) = await dao.TryGetRoomById(id);

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

    public async Task<List<IRoom>> GetByOwnerIdAsync(int ownerId, int amount)
    {
        var offlineRooms = await dao.GetByOwnerIdAsync(ownerId, amount, _rooms.Keys);
        var inMemoryRooms = _rooms.Values.Where(x => x.OwnerId == ownerId).ToList();
        
        return offlineRooms.Concat(inMemoryRooms).ToList();
    }

    public int Count => _rooms.Count;
    public IEnumerable<IRoom> GetAllRooms() => _rooms.Values;

    public async Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description)
    {
        return await dao.CreateRoomAsync(name, layoutId, ownerId, maxUsers, description);
    }

    public async Task<int> GetLayoutIdFromNameAsync(string name)
    {
        return await dao.GetLayoutIdFromNameAsync(name);
    }

    public async Task SaveRoomAsync(IRoom room)
    {
        await dao.SaveRoomAsync(room);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var room in _rooms.Values)
        {
            await dao.SaveRoomAsync(room);
            await room.DisposeAsync();
        }
        
        _rooms.Clear();
    }
}