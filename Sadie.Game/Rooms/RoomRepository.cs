using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.Db;

namespace Sadie.Game.Rooms;

public class RoomRepository(
    IDbContextFactory<SadieDbContext> dbContextFactory, 
    IMapper mapper) : IRoomRepository
{
    private readonly ConcurrentDictionary<long, IRoomLogic> _rooms = new();

    public IRoomLogic? TryGetRoomById(long id)
    {
        return _rooms.GetValueOrDefault(id);
    }

    public void AddRoom(IRoomLogic roomLogic) => _rooms[roomLogic.Room.Id] = roomLogic;

    public List<RoomDto> GetPopularRooms(int amount)
    {
        var popularRooms = _rooms
            .Values
            .Where(x => x.UserRepository.Count > 0)
            .OrderByDescending(x => x.UserRepository.Count)
            .Take(amount)
            .ToList();
        
        return mapper.Map<List<RoomDto>>(popularRooms);
    }

    public int Count => _rooms.Count;
    public IEnumerable<IRoomLogic> GetAllRooms() => _rooms.Values;

    public bool TryRemove(long id, out IRoomLogic? roomLogic)
    {
        return _rooms.TryRemove(id, out roomLogic);
    }
    
    public async ValueTask DisposeAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        foreach (var room in _rooms.Values)
        {
            dbContext.Entry(room).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            await room.DisposeAsync();
        }
        
        _rooms.Clear();
    }
}