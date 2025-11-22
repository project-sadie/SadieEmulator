using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomWallItemUpdated)]
public class RoomWallItemUpdatedEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    public int ItemId { get; init; }
    public string WallPosition { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        if (room == null || !client.RoomUser.HasRights())
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.PlayerFurnitureItem.Id == ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var wallPosition = WallPosition;

        if (string.IsNullOrEmpty(wallPosition))
        {
            return;
        }

        roomFurnitureItem.WallPosition = wallPosition;
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem).Property(x => x.WallPosition).IsModified = true;
        await dbContext.SaveChangesAsync();
        
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemUpdatedWriter
        {
            Item = roomFurnitureItem
        });
    }
}