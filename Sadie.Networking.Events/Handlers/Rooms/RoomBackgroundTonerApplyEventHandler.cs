using Sadie.Database;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomBackgroundTonerApply)]
public class RoomBackgroundTonerApplyEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    public required int Hue { get; set; }
    public required int Saturation { get; set; }
    public required int Brightness { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null ||
            client.RoomUser == null ||
            !client.RoomUser.HasRights())
        {
            return;
        }

        var roomFurnitureItem = client
            .RoomUser
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == ItemId);
        
        if (roomFurnitureItem == null)
        {
            return;
        }

        var metaData =
            $"{roomFurnitureItem.PlayerFurnitureItem.MetaData.Split(":")[0]}{Hue % 256}:{Saturation % 256}:{Brightness % 256}:";
        
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(client.RoomUser.Room, roomFurnitureItem, metaData);
        
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}