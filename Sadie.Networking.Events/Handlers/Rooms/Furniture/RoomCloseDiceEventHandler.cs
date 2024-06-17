using System.Drawing;
using Sadie.Database;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomCloseDice)]
public class RoomCloseDiceEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client
            .RoomUser
            .Room;
        
        var roomFurnitureItem = room
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

        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, roomFurnitureItem, "0");
        
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}