using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models;

namespace Sadie.Game.Rooms;

public class RoomRepository(SadieContext dbContext, IMapper mapper)
{
    private readonly ConcurrentDictionary<long, RoomLogic> _rooms = new();

    public Tuple<bool, RoomLogic?> TryGetRoomById(long id)
    {
        return new Tuple<bool, RoomLogic?>(_rooms.TryGetValue(id, out var room), room);
    }
    
    public Task<Tuple<bool, RoomLogic?>> TryLoadRoomByIdAsync(long id)
    {
        var (memoryResult, memoryValue) = TryGetRoomById(id);

        if (memoryResult)
        {
            return Task.FromResult(new Tuple<bool, RoomLogic?>(true, memoryValue));
        }

        var room = dbContext.Set<Room>()
            .Include(x => x.Owner)
            .Include(x => x.Layout)
            .Include(x => x.Settings)
            .Include(x => x.PaintSettings)
            .Include(x => x.PlayerRights)
            .Include(x => x.ChatMessages)
            .Include(x => x.FurnitureItems)
            .Include(x => x.PlayerLikes)
            .Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == id);

        if (room == null)
        {
            return Task.FromResult(new Tuple<bool, RoomLogic?>(false, null));
        }

        var roomLogic = mapper.Map<RoomLogic>(room);
        
        _rooms[room.Id] = roomLogic;

        return Task.FromResult(new Tuple<bool, RoomLogic?>(true, roomLogic));
    }

    public List<RoomLogic> GetPopularRooms(int amount)
    {
        return _rooms.Values.
            Where(x => x.UserRepository.Count > 0).
            OrderByDescending(x => x.UserRepository.Count).
            Take(amount).
            ToList();
    }

public async Task<List<RoomLogic>> GetAllByOwnerIdAsync(int ownerId, int amount)
{
    var rooms = await dbContext
        .Rooms
        .Where(x => x.OwnerId == ownerId)
        .Include(x => x.Owner)
        .Include(x => x.Layout)
        .Include(x => x.Settings)
        .Include(x => x.PaintSettings)
        .Include(x => x.PlayerRights)
        .Include(x => x.ChatMessages)
        .Include(x => x.FurnitureItems)
        .Include(x => x.PlayerLikes)
        .Include(x => x.Tags)
        .OrderByDescending(x => x.CreatedAt)
        .Take(amount)
        .ToListAsync();
    
    return mapper.Map<List<RoomLogic>>(rooms);
}

    public int Count => _rooms.Count;
    public IEnumerable<RoomLogic> GetAllRooms() => _rooms.Values;

    public async Task SaveRoomAsync(RoomLogic room)
    {
        dbContext.Rooms.Add(mapper.Map<Room>(room));
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var room in _rooms.Values)
        {
            await SaveRoomAsync(room);
            await room.DisposeAsync();
        }
        
        _rooms.Clear();
    }
}