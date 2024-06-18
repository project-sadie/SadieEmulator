using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomFurnitureItemUse)]
public class RoomItemUseEventHandler(
    RoomRepository roomRepository,
    RoomFurnitureItemInteractorRepository interactorRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);

        if (room == null)
        {
            return;
        }
        
        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.PlayerFurnitureItem.Id == ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor == null)
        {
            await RoomFurnitureItemHelpers.CycleInteractionStateForItemAsync(room, roomFurnitureItem, dbContext);
            return;
        }

        await interactor.OnTriggerAsync(room, roomFurnitureItem, client.RoomUser);
    }
}