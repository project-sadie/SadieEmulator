using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingMachineInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "vending_machine";
    
    public async Task OnTriggerAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
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
        
        await RoomFurnitureItemHelpers.CycleInteractionStateForItemAsync(room, item);
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUnit.Id,
            ItemId = handItems.PickRandom().Id
        });
    }

    public Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnPickUpAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnStepOnAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit? roomUnit) => Task.CompletedTask;
    public Task OnStepOffAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit? roomUnit) => Task.CompletedTask;
}