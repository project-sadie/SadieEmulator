using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Packets.Writers.Players.Inventory;
using Sadie.Networking.Packets.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomPaintItemPlaced)]
public class RoomPaintItemPlacedEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MissingRights);
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        if (room == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var playerItem = player.Player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        switch (playerItem.FurnitureItem.AssetName)
        {
            case "floor":
                room.Room.PaintSettings.FloorPaint = playerItem.MetaData;
                dbContext.Entry(room.Room.PaintSettings).Property(x => x.FloorPaint).IsModified = true;
                break;
            case "wallpaper":
                room.Room.PaintSettings.WallPaint = playerItem.MetaData;
                dbContext.Entry(room.Room.PaintSettings).Property(x => x.WallPaint).IsModified = true;
                break;
            case "landscape":
                room.Room.PaintSettings.LandscapePaint = playerItem.MetaData;
                dbContext.Entry(room.Room.PaintSettings).Property(x => x.LandscapePaint).IsModified = true;
                break;
        }

        player.Player.FurnitureItems.Remove(playerItem);
        await dbContext.SaveChangesAsync();
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
        {
            ItemId = ItemId
        });
        
        await room.BroadcastDataAsync(new RoomPaintWriter
        {
            Type = playerItem.FurnitureItem.AssetName,
            Value = playerItem.MetaData
        });
    }
}