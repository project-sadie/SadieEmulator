using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.Database;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemUse)]
public class RoomItemUseEventHandler(
    IRoomRepository roomRepository,
    IRoomFurnitureItemInteractorRepository interactorRepository,
    SadieContext dbContext,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || 
            client.RoomUser == null ||
            !client.RoomUser.HasRights())
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
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (!interactors.Any())
        {
            await roomFurnitureItemHelperService.CycleInteractionStateForItemAsync(room, roomFurnitureItem, dbContext);
        }
        else
        {
            foreach (var interactor in interactors)
            {
                await interactor.OnTriggerAsync(room, roomFurnitureItem, client.RoomUser);
            }
        }
    }
}