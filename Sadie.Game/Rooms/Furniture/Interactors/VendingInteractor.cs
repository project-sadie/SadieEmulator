using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Extensions;
using Sadie.Networking.Writers.Rooms.Users.HandItems;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingInteractor(IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.VendingMachine];
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementDataDto item, IRoomUser roomUser)
    {
        var direction = tileMapHelperService.GetOppositeDirection(item.Direction);

        roomUser.Direction = direction;
        roomUser.DirectionHead = direction;

        var handItems = item
            .PlayerFurnitureItem
            .FurnitureItem
            .HandItems
            .ToList();

        var squareInFront = tileMapHelperService.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (handItems.Count < 1 || roomUser.Point != squareInFront)
        {
            roomUser.WalkToPoint(squareInFront, OnReachedGoal);
            return;

            async void OnReachedGoal()
            {
                await OnTriggerAsync(room, item, roomUser);
            }
        }
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");
        await Task.Delay(500);
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");

        var handItem = handItems.PickRandom();

        roomUser.HandItemId = handItem.Id;
        roomUser.HandItemSet = DateTime.Now;
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUser.Player.Player.Id,
            ItemId = handItem.Id
        });
    }
}