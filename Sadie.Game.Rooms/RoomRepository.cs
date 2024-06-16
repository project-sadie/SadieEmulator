using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Rooms;

public class RoomRepository(SadieContext dbContext, IMapper mapper)
{
    private readonly ConcurrentDictionary<long, RoomLogic?> _rooms = new();

    public RoomLogic? TryGetRoomById(long id)
    {
        return _rooms.GetValueOrDefault(id);
    }

    public void AddRoom(RoomLogic roomLogic) => _rooms[roomLogic.Id] = roomLogic;

    public List<Room> GetPopularRooms(int amount)
    {
        return mapper.Map<List<Room>>(_rooms.Values.Where(x => x.UserRepository.Count > 0)
            .OrderByDescending(x => x.UserRepository.Count).Take(amount).ToList());
    }

    public int Count => _rooms.Count;
    public IEnumerable<RoomLogic?> GetAllRooms() => _rooms.Values;

    public bool TryRemove(long id, out RoomLogic? roomLogic)
    {
        return _rooms.TryRemove(id, out roomLogic);
    }
    
    public async ValueTask DisposeAsync()
    {
        foreach (var room in _rooms.Values)
        {
            dbContext.Entry(room).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            await room.DisposeAsync();
        }
        
        _rooms.Clear();
    }
}