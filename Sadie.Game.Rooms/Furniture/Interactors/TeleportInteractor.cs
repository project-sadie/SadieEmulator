using System.Drawing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Unit;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class TeleportInteractor(
    RoomRepository roomRepository,
    SadieContext dbContext,
    IMapper mapper) : AbstractRoomFurnitureItemInteractor
{
    public override string InteractionType => FurnitureItemInteractionType.Teleport;

    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(500);
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        var itemPosition = new Point(item.PositionX, item.PositionY);
        var itemInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUnit.Point == itemPosition)
        {
            roomUnit.CanWalk = false;
            
            var facingDirection = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        
            roomUnit.Direction = facingDirection;
            roomUnit.DirectionHead = facingDirection;

            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "1");
            await UseTeleportAsync(room, item, roomUnit);
        }
        else if (roomUnit.Point == itemInFront)
        {
            roomUnit.CanWalk = false;
            
            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "1");

            roomUnit.OverridePoints.Add(itemPosition);

            roomUnit.WalkToPoint(itemPosition, async () =>
            {
                roomUnit.OverridePoints.Remove(itemPosition);

                if (item.FurnitureItem.InteractionModes == 1)
                {
                    await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "2");
                    await Task.Delay(_delay);
                    await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
                }
                
                await UseTeleportAsync(room, item, roomUnit);
            });
        }
        else
        {
            roomUnit.WalkToPoint(itemInFront, OnReachedGoal);
            return;

            async void OnReachedGoal()
            {
                await OnTriggerAsync(room, item, roomUnit);
            }
        }
    }

    private async Task UseTeleportAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementData item,
        IRoomUnit roomUnit)
    {
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
                roomUnit,
                item,
                targetRoomItem,
                room);
        }
        else if (roomUnit.Type == RoomUnitType.User)
        {
            await UseTeleportInDifferentRoomAsync(
                (roomUnit as IRoomUser)!, 
                item,
                targetItemId,
                room);
        }
    }

    private async Task UseTeleportInDifferentRoomAsync(
        IRoomUser roomUser, 
        PlayerFurnitureItemPlacementData item,
        long targetItemId,
        IRoomLogic room)
    {
        var targetRoomId = await dbContext
            .RoomFurnitureItems
            .Where(x => x.PlayerFurnitureItemId == targetItemId)
            .Select(x => x.RoomId)
            .FirstOrDefaultAsync();

        if (targetRoomId != 0)
        {
            var targetRoom = await RoomHelpersDirty.TryLoadRoomByIdAsync(targetRoomId,
                roomRepository,
                dbContext,
                mapper);

            var targetItem = targetRoom?.FurnitureItems
                .FirstOrDefault(x => x.PlayerFurnitureItemId == targetItemId);

            if (targetItem != null)
            {
                roomUser.Player.State.Teleport = targetItem;
                
                await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(targetRoom!, targetItem, "2");
                await Task.Delay(_delay);
                    
                await roomUser.NetworkObject.WriteToStreamAsync(new RoomForwardEntryWriter
                {
                    RoomId = targetRoomId
                });
                
                await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(targetRoom!, targetItem, "1");
                await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
            }
        }
    }

    private async Task UseTeleportInSameRoomAsync(
        IRoomUnit roomUnit,
        PlayerFurnitureItemPlacementData item,
        PlayerFurnitureItemPlacementData targetItem,
        IRoomLogic room)
    {
        if (item.FurnitureItem.InteractionModes == 1)
        {
            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, targetItem, "2");
            await Task.Delay(_delay);
        }
        
        var newPoint = new Point(targetItem.PositionX, targetItem.PositionY);

        if (roomUnit.Type == RoomUnitType.User)
        {
            room.TileMap.UserMap[roomUnit.Point].Remove((IRoomUser) roomUnit);
            room.TileMap.AddUserToMap(newPoint, (IRoomUser) roomUnit);
        }
        else
        {
            room.TileMap.BotMap[roomUnit.Point].Remove((IRoomBot) roomUnit);
            room.TileMap.AddBotToMap(newPoint, (IRoomBot) roomUnit);
        }

        roomUnit.Point = newPoint;
        roomUnit.Direction = targetItem.Direction;
        roomUnit.DirectionHead = targetItem.Direction;
            
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, targetItem, "1");
        
        var squareInFront = RoomTileMapHelpers.GetPointInFront(targetItem.PositionX, targetItem.PositionY, targetItem.Direction);

        roomUnit.WalkToPoint(squareInFront, OnReachedGoal);
        return;

        async void OnReachedGoal()
        {
            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
            await Task.Delay(_delay);
            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, targetItem, "0");
            
            roomUnit.CanWalk = true;
        }
    }
}