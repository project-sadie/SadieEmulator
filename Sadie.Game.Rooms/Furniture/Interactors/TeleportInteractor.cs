using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class TeleportInteractor(
    RoomRepository roomRepository,
    SadieContext dbContext) : IRoomFurnitureItemInteractor
{
    public string InteractionType => "teleport";
    
    public async Task OnClickAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        var itemPosition = new Point(item.PositionX, item.PositionY);
        
        if (roomUser.Point != itemPosition)
        {
            roomUser.WalkToPoint(itemPosition, OnReachedGoal);
            return;

            async void OnReachedGoal()
            {
                await OnClickAsync(room, item, roomUser);
            }
        }
        
        var facingDirection = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        
        roomUser.Direction = facingDirection;
        roomUser.DirectionHead = facingDirection;
        
        item.MetaData = "1";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
        
        var link = await dbContext
            .RoomFurnitureItemTeleportLinks
            .Where(x => x.ParentId == item.Id || x.ChildId == item.Id)
            .FirstOrDefaultAsync();

        if (link == null)
        {
            return;
        }

        var targetItemId = link.ParentId == item.Id ? link.ChildId : link.ParentId;
        
        var targetRoomItem = room
            .FurnitureItems
            .FirstOrDefault(x => x.Id == targetItemId);
        
        if (targetRoomItem != null)
        {
            await UseTeleportInSameRoomAsync(
                roomUser, 
                item,
                targetRoomItem,
                room);
        }
        else
        {
            await UseTeleportInDifferentRoomAsync(
                roomUser,
                item,
                targetItemId,
                room);
        }
    }

    private async Task UseTeleportInDifferentRoomAsync(
        IRoomUser roomUser, 
        RoomFurnitureItem item,
        long targetItemId,
        IRoomLogic room)
    {
        item.MetaData = "0";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
            
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
                targetItem.MetaData = "1";
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(targetRoom!, targetItem);
                    
                roomUser.Player.State.Teleport = targetItem;
                    
                await roomUser.NetworkObject.WriteToStreamAsync(new RoomForwardEntryWriter
                {
                    RoomId = targetRoomId
                });
            }
        }
    }

    private async Task UseTeleportInSameRoomAsync(
        IRoomUser roomUser, 
        RoomFurnitureItem item,
        RoomFurnitureItem targetItem,
        IRoomLogic room)
    {
        var newPoint = new Point(targetItem.PositionX, targetItem.PositionY);
            
        room.TileMap.UserMap[roomUser.Point].Remove(roomUser);
        room.TileMap.AddUserToMap(newPoint, roomUser);

        roomUser.Point = newPoint;
        roomUser.Direction = targetItem.Direction;
        roomUser.DirectionHead = targetItem.Direction;

        item.MetaData = "0";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
            
        targetItem.MetaData = "1";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, targetItem);
            
        var squareInFront = RoomTileMapHelpers.GetPointInFront(targetItem.PositionX, targetItem.PositionY, targetItem.Direction);

        roomUser.WalkToPoint(squareInFront, OnReachedGoal);
        return;

        async void OnReachedGoal()
        {
            targetItem.MetaData = "0";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, targetItem);
        }
    }
}