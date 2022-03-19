using System.Collections.Concurrent;

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

    public async Task RunPeriodicCheckAsync()
    {
        foreach (var room in _rooms.Values)
        {
            await room.RunPeriodicCheckAsync();
        }
    }

    public List<IRoom> GetAll()
    {
        return _rooms.Values.ToList();
    }

    public void Dispose()
    {
        foreach (var room in _rooms.Values)
        {
            room?.Dispose();
        }
        
        _rooms.Clear();
    }
}