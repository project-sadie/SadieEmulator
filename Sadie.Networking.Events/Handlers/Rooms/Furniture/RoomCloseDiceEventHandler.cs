using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomCloseDice)]
public class RoomCloseDiceEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client
            .RoomUser
            .Room;
        
        var roomFurnitureItem = room
            .Room.FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);

        if (roomFurnitureItem == null || roomFurnitureItem.PlayerFurnitureItem.MetaData == "-1")
        {
            return;
        }

        var itemPosition = new Point(roomFurnitureItem.PositionX, roomFurnitureItem.PositionY);
        
        if (tileMapHelperService.GetSquaresBetweenPoints(itemPosition, client.RoomUser.Point) > 1)
        {
            return;
        }

        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, roomFurnitureItem, "0");
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}