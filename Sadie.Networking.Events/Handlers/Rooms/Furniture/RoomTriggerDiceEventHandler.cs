using System.Drawing;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomTriggerDice)]
public class RoomTriggerDiceEventHandler(IRoomFurnitureItemInteractorRepository interactorRepository,
    IRoomTileMapHelperService tileMapHelperService) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var roomFurnitureItem = client
            .RoomUser
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);

        if (roomFurnitureItem == null || roomFurnitureItem.PlayerFurnitureItem!.MetaData == "-1")
        {
            return;
        }

        var itemPosition = new Point(roomFurnitureItem.PositionX, roomFurnitureItem.PositionY);
        
        if (tileMapHelperService.GetSquaresBetweenPoints(itemPosition, client.RoomUser.Point) > 1)
        {
            return;
        }
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);
        
        foreach (var interactor in interactors)
        {
            await interactor.OnTriggerAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
        }
    }
}