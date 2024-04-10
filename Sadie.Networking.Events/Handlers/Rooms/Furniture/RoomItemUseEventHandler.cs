using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomItemUseEventHandler(
    RoomFurnitureItemUseEventParser eventParser,
    RoomRepository roomRepository,
    RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomFurnitureItemUse;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);

        if (room == null)
        {
            return;
        }
        
        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.Id == eventParser.ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor == null)
        {
            return;
        }

        await interactor.OnClickAsync(room, roomFurnitureItem, client.Player);
    }
}