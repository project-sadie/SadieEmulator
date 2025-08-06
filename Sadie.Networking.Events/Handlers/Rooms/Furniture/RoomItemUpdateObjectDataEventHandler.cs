using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemUpdateObjectData)]
public class RoomItemUpdateObjectDataEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    public required Dictionary<string, string> ObjectData { get; init; }
    
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
            .FirstOrDefault(x => x.Id == ItemId);
        
        if (roomFurnitureItem == null)
        {
            return;
        }

        var metaData = string.Join(";", ObjectData.Select(x => $"{x.Key}={x.Value}"));
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(client.RoomUser.Room, roomFurnitureItem, metaData);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}