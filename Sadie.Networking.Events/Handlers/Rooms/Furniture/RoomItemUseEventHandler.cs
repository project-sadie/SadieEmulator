using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomFurnitureItemUse)]
public class RoomItemUseEventHandler(
    RoomRepository roomRepository,
    RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);

        if (room == null)
        {
            return;
        }
        
        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor == null)
        {
            return;
        }

        await interactor.OnClickAsync(room, roomFurnitureItem, client.RoomUser);
    }
}