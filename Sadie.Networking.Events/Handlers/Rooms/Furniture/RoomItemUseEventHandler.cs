using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemUse)]
public class RoomItemUseEventHandler(
    IRoomFurnitureItemInteractorRepository interactorRepository,
    SadieContext dbContext,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IRoomWiredService wiredService) : INetworkPacketEventHandler
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

        await RoomHelpers.UseRoomFurnitureAsync(
            room,
            client.RoomUser,
            roomFurnitureItem,
            interactorRepository, 
            roomFurnitureItemHelperService, 
            dbContext, 
            wiredService);
    }
}