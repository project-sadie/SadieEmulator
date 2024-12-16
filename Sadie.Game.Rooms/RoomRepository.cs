using System.Collections.Concurrent;
using AutoMapper;
using Sadie.API.Game.Rooms;
using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Rooms;

public class RoomRepository(
    IMapper mapper,
    ConcurrentDictionary<int, IRoomLogic> rooms)
    : IRoomRepository
{
    public IRoomLogic? TryGetRoomById(int id)
    {
        return rooms.GetValueOrDefault(id);
    }

    public void AddRoom(IRoomLogic roomLogic) => 
        rooms[roomLogic.Id] = roomLogic;

    public List<Room> GetPopularRooms(int amount)
    {
        return mapper.Map<List<Room>>(rooms.Values.Where(x => x.UserRepository.Count > 0)
            .OrderByDescending(x => x.UserRepository.Count).Take(amount).ToList());
    }

    public int Count => rooms.Count;
    
    public IEnumerable<IRoomLogic> GetAllRooms() => 
        rooms.Values;

    public bool TryRemove(int id, out IRoomLogic? roomLogic)
    {
        return rooms.TryRemove(id, out roomLogic);
    }
    
    public ValueTask DisposeAsync()
    {
        rooms.Clear();
        return ValueTask.CompletedTask;
    }
}