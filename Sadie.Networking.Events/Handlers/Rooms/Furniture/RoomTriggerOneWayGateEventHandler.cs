using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomTriggerOneWayGate)]
public class RoomTriggerOneWayGateEventHandler(IRoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var roomFurnitureItem = client
            .RoomUser
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);
        
        if (roomFurnitureItem is not { PlayerFurnitureItem.MetaData: "0" })
        {
            return;
        }
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem
                .PlayerFurnitureItem
                .FurnitureItem.InteractionType ?? "");
        
        foreach (var interactor in interactors)
        {
            await interactor.OnTriggerAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
        }
    }
}