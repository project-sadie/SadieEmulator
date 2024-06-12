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
        
        await RoomFurnitureItemHelpers.CycleInteractionStateForItemAsync(room, item);
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUser.Id,
            ItemId = handItems.PickRandom().Id
        });
    }

    public Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser) => Task.CompletedTask;
    public Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser) => Task.CompletedTask;
}