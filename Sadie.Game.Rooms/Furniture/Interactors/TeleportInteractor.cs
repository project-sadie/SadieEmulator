using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Rooms.Unit;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class TeleportInteractor(
    RoomRepository roomRepository,
    SadieContext dbContext) : IRoomFurnitureItemInteractor
{
    public string InteractionType => "teleport";
    
    public async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        var itemPosition = new Point(item.PositionX, item.PositionY);
        var itemInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUnit.Point == itemPosition)
        {
            var facingDirection = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        
            roomUnit.Direction = facingDirection;
            roomUnit.DirectionHead = facingDirection;
            
            item.PlayerFurnitureItem!.MetaData = "1";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);

            await UseTeleportAsync(room, item, roomUnit);
        }
        else if (roomUnit.Point == itemInFront)
        {
            item.PlayerFurnitureItem!.MetaData = "1";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);

            roomUnit.OverridePoints.Add(itemPosition);

            roomUnit.WalkToPoint(itemPosition, async () =>
            {
                roomUnit.OverridePoints.Remove(itemPosition);
                
                item.PlayerFurnitureItem.MetaData = "0";
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
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
            .Where(x => x.ParentId == item.PlayerFurnitureItem.Id || x.ChildId == item.PlayerFurnitureItem.Id)
            .FirstOrDefaultAsync();

        if (link == null)
        {
            return;
        }

        var targetItemId = link.ParentId == item.PlayerFurnitureItem.Id ? link.ChildId : link.ParentId;
        
        var targetRoomItem = room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItem.Id == targetItemId);

        Task.Factory.StartNew(async () =>
        {
            await Task.Delay(800);
                
            item.PlayerFurnitureItem.MetaData = "0";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
        });
        
        if (targetRoomItem != null)
        {
            await UseTeleportInSameRoomAsync(
                roomUnit,
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

    public Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;

    private async Task UseTeleportInDifferentRoomAsync(
        IRoomUser roomUser, 
        PlayerFurnitureItemPlacementData item,
        long targetItemId,
        IRoomLogic room)
    {
        var targetRoomId = await dbContext
            .RoomFurnitureItems
            .Where(x => x.Id == targetItemId)
            .Select(x => x.RoomId)
            .FirstOrDefaultAsync();

        if (targetRoomId != 0)
        {
            var targetRoom = await roomRepository.TryLoadRoomByIdAsync(targetRoomId);

            var targetItem = targetRoom?.FurnitureItems
                .FirstOrDefault(x => x.Id == targetItemId);

            if (targetItem != null)
            {
                roomUser.Player.State.Teleport = targetItem;
                    
                await roomUser.NetworkObject.WriteToStreamAsync(new RoomForwardEntryWriter
                {
                    RoomId = targetRoomId
                });
                
                targetItem.PlayerFurnitureItem.MetaData = "1";
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(targetRoom!, targetItem);
            }
        }
    }

    private async Task UseTeleportInSameRoomAsync(
        IRoomUnit roomUnit,
        PlayerFurnitureItemPlacementData targetItem,
        IRoomLogic room)
    {
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
            
        targetItem.PlayerFurnitureItem.MetaData = "1";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, targetItem);
            
        var squareInFront = RoomTileMapHelpers.GetPointInFront(targetItem.PositionX, targetItem.PositionY, targetItem.Direction);

        roomUnit.WalkToPoint(squareInFront, OnReachedGoal);
        return;

        async void OnReachedGoal()
        {
            targetItem.PlayerFurnitureItem.MetaData = "0";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, targetItem);
        }
    }

    public Task OnMoveAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnStepOnAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
    public Task OnStepOffAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
}