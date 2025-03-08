using System.Drawing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Players;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Unsorted;

namespace Sadie.Game.Rooms;

public static class RoomHelpers
{
    public static async Task<IRoomLogic?> TryLoadRoomByIdAsync(
        long id, 
        IRoomRepository roomRepository, 
        SadieContext dbContext,
        IMapper mapper)
    {
        var memoryValue = roomRepository.TryGetRoomById(id);

        if (memoryValue != null)
        {
            return memoryValue;
        }

        var room = await dbContext.Set<Room>()
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
            player.Id,
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
        SadieContext dbContext)
    {
        var roomVisit = new PlayerRoomVisit
        {
            PlayerId = player.Id,
            RoomId = roomId,
            CreatedAt = DateTime.Now
        };
        
        player.RoomVisits.Add(roomVisit);

        dbContext.PlayerRoomVisits.Add(roomVisit);
        await dbContext.SaveChangesAsync();
    }
    
    public static async Task UseRoomFurnitureAsync(
        IRoomLogic room,
        IRoomUser user,
        PlayerFurnitureItemPlacementData roomFurnitureItem,
        IRoomFurnitureItemInteractorRepository interactorRepository,
        IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
        SadieContext dbContext,
        IRoomWiredService wiredService)
    {
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactors.Count == 0)
        {
            await roomFurnitureItemHelperService.CycleInteractionStateForItemAsync(
                room, 
                roomFurnitureItem, 
                dbContext);
        }
        else
        {
            foreach (var interactor in interactors)
            {
                await interactor.OnTriggerAsync(room, roomFurnitureItem, user!);
            }
        }
        
        var matchingWiredTriggers = wiredService.GetTriggers(
            FurnitureItemInteractionType.WiredTriggerFurnitureStateChanged,
            room.FurnitureItems,"", [roomFurnitureItem.Id]);

        foreach (var trigger in matchingWiredTriggers)
        {
            await wiredService.RunTriggerForRoomAsync(room, trigger, user!);
        }
    }
}