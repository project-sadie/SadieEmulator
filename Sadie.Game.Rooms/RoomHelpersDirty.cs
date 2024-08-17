using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Rooms;

public static class RoomHelpersDirty
{
    public static async Task<IRoomLogic?> TryLoadRoomByIdAsync(
        long id, 
        RoomRepository roomRepository, 
        SadieContext dbContext,
        IMapper mapper)
    {
        var memoryValue = roomRepository.TryGetRoomById(id);

        if (memoryValue != null)
        {
            return memoryValue;
        }

        var room = await dbContext.Set<Room>()
            .Include(x => x.FurnitureItems)
                .ThenInclude(x => x.PlayerFurnitureItem)
                .ThenInclude(x => x.FurnitureItem)
            .Include(x => x.FurnitureItems)
                .ThenInclude(x => x.PlayerFurnitureItem)
                .ThenInclude(x => x.Player)
            .Include(x => x.PlayerLikes)
            .Include(x => x.Tags)
            .Include(x => x.Group)
            .Include(x => x.DimmerSettings)
            .Include(x => x.PlayerBans).ThenInclude(x => x.Player)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (room == null)
        {
            return null;
        }

        var roomLogic = mapper.Map<RoomLogic>(room);
        roomRepository.AddRoom(roomLogic);

        return roomLogic;
    }
}