using System.Drawing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Rooms;

namespace Sadie.Networking.Events;

public static class RoomHelpers
{
    public static async Task<IRoomLogic?> TryLoadRoomByIdAsync(
        long id, 
        IRoomRepository roomRepository, 
        IDbContextFactory<SadieDbContext> dbContextFactory,
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

        var roomDto = mapper.Map<RoomDto>(room);
        var roomLogic = mapper.Map<IRoomLogic>(roomDto);

        roomLogic.UserRepository.SetRoom(roomLogic);
        roomRepository.AddRoom(roomLogic);

        return roomLogic;
    }
    
    private static RoomControllerLevel GetControllerLevelForUser(IRoomLogic room, IPlayerLogic player)
    {
        var controllerLevel = RoomControllerLevel.None;
        
        if (room.Room.PlayerRights.FirstOrDefault(x => x.PlayerId == player.Player.Id) != null)
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.Room.OwnerId == player.Player.Id)
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
        IDbContextFactory<SadieDbContext> dbContextFactory,
        IMapper mapper)
    {
        var roomVisit = new PlayerRoomVisitDto
        {
            PlayerId = player.Player.Id,
            RoomId = roomId,
            CreatedAt = DateTime.Now
        };
        
        player.Player.RoomVisits.Add(roomVisit);
        
        var roomVisitEntity = mapper.Map<PlayerRoomVisit>(roomVisit);

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerRoomVisits.Add(roomVisitEntity);
        await dbContext.SaveChangesAsync();
    }
}