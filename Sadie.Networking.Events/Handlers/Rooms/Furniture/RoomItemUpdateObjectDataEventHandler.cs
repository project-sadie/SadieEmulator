using Sadie.Database;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemUpdateObjectData)]
public class RoomItemUpdateObjectDataEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    public required Dictionary<string, string> ObjectData { get; set; }
    
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

        var metaData = string.Join(";", ObjectData.Select(x => $"{x.Key}={x.Value}"));
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(client.RoomUser.Room, roomFurnitureItem, metaData);
        
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}