using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.Db;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Attributes;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemUse)]
public class RoomItemUseEventHandler(
    IRoomFurnitureItemInteractorRepository interactorRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public int ItemId { get; init; }
    
    [RequiresRoomRights]
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser!.Room;

        var roomFurnitureItem = room
                .FurnitureItems
                .FirstOrDefault(x => x.PlayerFurnitureItemId == ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (!interactors.Any())
        {
            await roomFurnitureItemHelperService.CycleInteractionStateForItemAsync(room, roomFurnitureItem, dbContextFactory);
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