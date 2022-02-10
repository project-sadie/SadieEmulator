using System.Collections.Concurrent;

namespace Sadie.Game.Rooms;

public class RoomRepository : IRoomRepository
{
    private readonly IRoomDao _dao;
    private readonly ConcurrentDictionary<long, RoomEntity> _rooms;

    public RoomRepository(IRoomDao dao, ConcurrentDictionary<long, RoomEntity> rooms)
    {
        _dao = dao;
        _rooms = rooms;
    }
    
    public Tuple<bool, RoomEntity?> TryGetRoomById(long id)
    {
        return new Tuple<bool, RoomEntity?>(_rooms.TryGetValue(id, out var room), room);
    }
    
    public async Task<Tuple<bool, RoomEntity?>> TryLoadRoomByIdAsync(long id)
    {
        var (result, room) = await _dao.TryGetRoomById(id);

        if (result && room != null)
        {
            _rooms[room.Id] = room;
            return await _dao.TryGetRoomById(id);
        }

        return new Tuple<bool, RoomEntity?>(false, null);
    }
}