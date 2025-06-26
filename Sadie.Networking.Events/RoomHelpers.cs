using System.Drawing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Rooms;
using Sadie.Enums.Game.Players;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Unsorted;

namespace Sadie.Networking.Events;

public static class RoomHelpers
{
    public static async Task<IRoomLogic?> TryLoadRoomByIdAsync(
        long id, 
        IRoomRepository roomRepository, 
        IDbContextFactory<SadieContext> dbContextFactory,
        IMapper mapper)
    {
        var memoryValue = roomRepository.TryGetRoomById(id);

        if (memoryValue != null)
        {
            return memoryValue;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var room = await dbContext.Set<Room>()
            .Include(x => x.Layout)
            .Include(x => x.FurnitureItems)
            .Include(x => x.Owner)
            .Include(x => x.PaintSettings)
            .Include(x => x.ChatSettings)
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

        var roomLogic = mapper.Map<IRoomLogic>(room);
        roomRepository.AddRoom(roomLogic);

        return roomLogic;
    }
    
    private static RoomControllerLevel GetControllerLevelForUser(IRoom room, IPlayerLogic player)
    {
        var controllerLevel = RoomControllerLevel.None;
        
        if (room.PlayerRights.FirstOrDefault(x => x.PlayerId == player.Id) != null)
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.OwnerId == player.Id)
        {
            controllerLevel = RoomControllerLevel.Owner;
        }

        if (player.HasPermission(PlayerPermissionName.AnyRoomRights))
        {
            controllerLevel = RoomControllerLevel.Owner;
        }

        if (player.HasPermission(PlayerPermissionName.Moderator))
        {
            controllerLevel = RoomControllerLevel.Moderator;
        }

        if (player.HasPermission(PlayerPermissionName.AnyRoomRights))
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        return controllerLevel;
    }

    public static IRoomUser CreateUserForEntry(
        IRoomUserFactory roomUserFactory, 
        IRoomLogic room, 
        IPlayerLogic player,
        Point spawnPoint,
        HDirection direction)
    {
        return roomUserFactory.Create(
            room,
            player.NetworkObject!,
            spawnPoint,
            room.TileMap.ZMap[spawnPoint.Y, spawnPoint.X],
            direction,
            direction,
            player,
            GetControllerLevelForUser(room, player));
    }
    
    public static async Task CreateRoomVisitForPlayerAsync(
        IPlayerLogic player, 
        int roomId, 
        IDbContextFactory<SadieContext> dbContextFactory)
    {
        var roomVisit = new PlayerRoomVisit
        {
            PlayerId = player.Id,
            RoomId = roomId,
            CreatedAt = DateTime.Now
        };
        
        player.RoomVisits.Add(roomVisit);

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerRoomVisits.Add(roomVisit);
        await dbContext.SaveChangesAsync();
    }
}