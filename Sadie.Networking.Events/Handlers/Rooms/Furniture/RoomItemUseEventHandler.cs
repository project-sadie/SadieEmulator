using Sadie.API.Game.Rooms.Furniture;
using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemUse)]
public class RoomItemUseEventHandler(
    IRoomFurnitureItemInteractorRepository interactorRepository,
    SadieContext dbContext,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    
    [RequiresRoomRights]
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser!.Room;

        var roomFurnitureItem = room
                .FurnitureItems
                .FirstOrDefault(x => x.Id == ItemId);

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