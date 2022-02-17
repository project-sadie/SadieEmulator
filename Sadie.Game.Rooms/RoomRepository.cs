using System.Collections.Concurrent;

namespace Sadie.Game.Rooms;

public class RoomRepository : IRoomRepository
{
    private readonly IRoomDao _dao;
    private readonly ConcurrentDictionary<long, Room> _rooms;

    public RoomRepository(IRoomDao dao)
    {
        _dao = dao;
        _rooms = new ConcurrentDictionary<long, Room>();
    }
    
    public Tuple<bool, Room?> TryGetRoomById(long id)
    {
        return new Tuple<bool, Room?>(_rooms.TryGetValue(id, out var room), room);
    }
    
    public async Task<Tuple<bool, Room?>> TryLoadRoomByIdAsync(long id)
    {
        var (memoryResult, memoryValue) = TryGetRoomById(id);

        if (memoryResult)
        {
            return new Tuple<bool, Room?>(true, memoryValue);
        }
        
        var (result, room) = await _dao.TryGetRoomById(id);

        if (result && room != null)
        {
            _rooms[room.Id] = room;
        }

        return TryGetRoomById(id);
    }
}