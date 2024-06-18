using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingInteractor : AbstractRoomFurnitureItemInteractor
{
    public override string InteractionType => "vending_machine";
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        var direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);

        roomUnit.Direction = direction;
        roomUnit.DirectionHead = direction;

        var handItems = item
            .FurnitureItem
            .HandItems
            .ToList();

        var squareInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (handItems.Count < 1 || roomUnit.Point != squareInFront)
        {
            roomUnit.WalkToPoint(squareInFront, OnReachedGoal);
            return;

            async void OnReachedGoal()
            {
                await OnTriggerAsync(room, item, roomUnit);
            }
        }
        
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "1");
        await Task.Delay(500);
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUnit.Id,
            ItemId = handItems.PickRandom().Id
        });
    }
}