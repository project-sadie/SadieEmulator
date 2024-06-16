using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomTriggerOneWayGate)]
public class RoomTriggerOneWayGateEventHandler(RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var roomFurnitureItem = client
            .RoomUser
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == ItemId);
        
        if (roomFurnitureItem is not { PlayerFurnitureItem.MetaData: "0" })
        {
            // return;
        }
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnTriggerAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
        }
    }
}