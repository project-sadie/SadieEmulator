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
    
    public async Task<RoomLogic?> TryLoadRoomByIdAsync(long id)
    {
        var memoryValue = TryGetRoomById(id);

        if (memoryValue != null)
        {
            return memoryValue;
        }

        var room = await dbContext.Set<Room>()
            .Include(x => x.Owner)
            .Include(x => x.Layout)
            .Include(x => x.Settings)
            .Include(x => x.PaintSettings)
            .Include(x => x.ChatSettings)
            .Include(x => x.PlayerRights)
            .Include(x => x.ChatMessages)
            .Include(x => x.FurnitureItems).ThenInclude(x => x.FurnitureItem)
            .Include(x => x.PlayerLikes)
            .Include(x => x.Tags)
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (room == null)
        {
            return null;
        }

        var roomLogic = mapper.Map<RoomLogic>(room);
        _rooms[room.Id] = roomLogic;

        return roomLogic;
    }

    public void AddRoom(Room room) => _rooms[room.Id] = mapper.Map<RoomLogic>(room);

    public List<Room> GetPopularRooms(int amount)
    {
        return mapper.Map<List<Room>>(_rooms.Values.Where(x => x.UserRepository.Count > 0)
            .OrderByDescending(x => x.UserRepository.Count).Take(amount).ToList());
    }

    public int Count => _rooms.Count;
    public IEnumerable<RoomLogic?> GetAllRooms() => _rooms.Values;

    public async Task SaveRoomAsync(RoomLogic? room)
    {
        dbContext.Rooms.Add(mapper.Map<Room>(room));
        await dbContext.SaveChangesAsync();
    }

    public bool TryUnloadRoom(long id, out RoomLogic? roomLogic)
    {
        return _rooms.TryRemove(id, out roomLogic);
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