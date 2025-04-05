using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingInteractor(IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.VendingMachine];
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var direction = tileMapHelperService.GetOppositeDirection((int) item.Direction);

        roomUser.Direction = direction;
        roomUser.DirectionHead = direction;

        var handItems = item
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
            UserId = roomUser.Player.Id,
            ItemId = handItem.Id
        });
    }
}