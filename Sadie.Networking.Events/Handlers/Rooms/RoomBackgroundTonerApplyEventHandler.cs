using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.Db;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Attributes;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomBackgroundTonerApply)]
public class RoomBackgroundTonerApplyEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    public required int Hue { get; init; } 
    public required int Saturation { get; init; }
    public required int Brightness { get; init; }
    
    [RequiresRoomRights]
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }

        var roomFurnitureItem = client
            .RoomUser!
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);
        
        if (roomFurnitureItem == null)
        {
            return;
        }

        var metaData =
            $"{roomFurnitureItem.PlayerFurnitureItem.MetaData.Split(":")[0]}{Hue % 256}:{Saturation % 256}:{Brightness % 256}:";
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(client.RoomUser.Room, roomFurnitureItem, metaData);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}