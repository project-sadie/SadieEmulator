using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingMachineInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "vending_machine";
    
    public async Task OnClickAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        item.MetaData = "0";

        var direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);

        roomUser.Direction = direction;
        roomUser.DirectionHead = direction;

        var handItems = item
            .FurnitureItem
            .HandItems
            .ToList();

        var squareInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (handItems.Count < 1 || roomUser.Point != squareInFront)
        {
            roomUser.WalkToPoint(squareInFront, OnReachedGoal);
            return;

            async void OnReachedGoal()
            {
                await OnClickAsync(room, item, roomUser);
            }
        }
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUser.Id,
            ItemId = handItems.PickRandom().Id
        });

        var itemWriter = new RoomFloorItemUpdatedWriter
        {
            Id = item.Id,
            AssetId = item.FurnitureItem.AssetId,
            PositionX = item.PositionX,
            PositionY = item.PositionY,
            Direction = (int)item.Direction,
            PositionZ = item.PositionZ,
            StackHeight = 0.ToString(),
            Extra = 0,
            ObjectDataKey = 0,
            ExtraData = item.MetaData,
            Expires = -1,
            InteractionModes = 1,
            OwnerId = item.OwnerId
        };
        
        await room.UserRepository.BroadcastDataAsync(itemWriter);
    }
}