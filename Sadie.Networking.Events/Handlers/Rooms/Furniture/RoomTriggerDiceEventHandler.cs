using System.Drawing;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomTriggerDice)]
public class RoomTriggerDiceEventHandler(RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var roomFurnitureItem = client
            .RoomUser
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == ItemId);

        if (roomFurnitureItem == null || roomFurnitureItem.PlayerFurnitureItem!.MetaData == "-1")
        {
            return;
        }

        var itemPosition = new Point(roomFurnitureItem.PositionX, roomFurnitureItem.PositionY);
        
        if (RoomTileMapHelpers.GetTilesBetween(itemPosition, client.RoomUser.Point) > 1)
        {
            return;
        }
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnTriggerAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
        }
    }
}