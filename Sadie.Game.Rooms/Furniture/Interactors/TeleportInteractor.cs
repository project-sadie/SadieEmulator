using System.Drawing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class TeleportInteractor(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
    IMapper mapper,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.Teleport];

    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(500);
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var itemPosition = new Point(item.PositionX, item.PositionY);
        var itemInFront = tileMapHelperService.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUser.Point == itemPosition)
        {
            roomUser.CanWalk = false;
            
            var facingDirection = tileMapHelperService.GetOppositeDirection((int) item.Direction);
        
            roomUser.Direction = facingDirection;
            roomUser.DirectionHead = facingDirection;

            await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");
            await UseTeleportAsync(room, item, roomUser);
        }
        else if (roomUser.Point == itemInFront)
        {
            roomUser.CanWalk = false;
            
            await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");

            roomUser.OverridePoints.Add(itemPosition);

            roomUser.WalkToPoint(itemPosition, async void () =>
            {
                roomUser.OverridePoints.Remove(itemPosition);

                if (item.FurnitureItem.InteractionModes == 1)
                {
                    await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "2");
                    await Task.Delay(_delay);
                    await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
                }
                
                await UseTeleportAsync(room, item, roomUser);
            });
        }
        else
        {
            roomUser.WalkToPoint(itemInFront, OnReachedGoal);
            return;

            async void OnReachedGoal()
            {
                await OnTriggerAsync(room, item, roomUser);
            }
        }
    }

    private async Task UseTeleportAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementData item,
        IRoomUser roomUser)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var link = await dbContext
            .PlayerFurnitureItemLinks
            .Where(x => x.ParentId == item.PlayerFurnitureItemId || x.ChildId == item.PlayerFurnitureItemId)
            .FirstOrDefaultAsync();

        if (link == null)
        {
            return;
        }

        var targetItemId = link.ParentId == item.PlayerFurnitureItemId ? link.ChildId : link.ParentId;
        
        var targetRoomItem = room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == targetItemId);
        
        if (targetRoomItem != null)
        {
            await UseTeleportInSameRoomAsync(
                roomUser,
                item,
                targetRoomItem,
                room);
        }
        
        await UseTeleportInDifferentRoomAsync(
            roomUser, 
            item,
            targetItemId,
            room);
    }

    private async Task UseTeleportInDifferentRoomAsync(
        IRoomUser roomUser, 
        PlayerFurnitureItemPlacementData item,
        long targetItemId,
        IRoomLogic room)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var targetRoomId = await dbContext
            .RoomFurnitureItems
            .Where(x => x.PlayerFurnitureItemId == targetItemId)
            .Select(x => x.RoomId)
            .FirstOrDefaultAsync();

        if (targetRoomId != 0)
        {
            var targetRoom = await RoomHelpers.TryLoadRoomByIdAsync(targetRoomId,
                roomRepository,
                dbContextFactory,
                mapper);

            var targetItem = targetRoom?.FurnitureItems
                .FirstOrDefault(x => x.PlayerFurnitureItemId == targetItemId);

            if (targetItem != null)
            {
                roomUser.Player.State.Teleport = targetItem;
                
                await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(targetRoom!, targetItem, "2");
                await Task.Delay(_delay);
                    
                await roomUser.NetworkObject.WriteToStreamAsync(new RoomForwardEntryWriter
                {
                    RoomId = targetRoomId
                });
                
                await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(targetRoom!, targetItem, "1");
                await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
            }
        }
    }

    private async Task UseTeleportInSameRoomAsync(
        IRoomUser roomUser,
        PlayerFurnitureItemPlacementData item,
        PlayerFurnitureItemPlacementData targetItem,
        IRoomLogic room)
    {
        if (item.FurnitureItem.InteractionModes == 1)
        {
            await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, targetItem, "2");
            await Task.Delay(_delay);
        }
        
        var newPoint = new Point(targetItem.PositionX, targetItem.PositionY);

        room.TileMap.UnitMap[roomUser.Point].Remove(roomUser);
        room.TileMap.AddUnitToMap(newPoint, roomUser);

        await roomUser.SetPositionAsync(newPoint);
        
        roomUser.Direction = targetItem.Direction;
        roomUser.DirectionHead = targetItem.Direction;
            
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, targetItem, "1");
        
        var squareInFront = tileMapHelperService.GetPointInFront(targetItem.PositionX, targetItem.PositionY, targetItem.Direction);

        roomUser.WalkToPoint(squareInFront, OnReachedGoal);
        return;

        async void OnReachedGoal()
        {
            await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
            await Task.Delay(_delay);
            await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, targetItem, "0");
            
            roomUser.CanWalk = true;
        }
    }
}