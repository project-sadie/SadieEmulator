using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomPaintItemPlaced)]
public class RoomPaintItemPlacedEventHandler(
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        
        if (room == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        switch (playerItem.FurnitureItem.AssetName)
        {
            case "floor":
                room.PaintSettings.FloorPaint = playerItem.MetaData;
                dbContext.Entry(room.PaintSettings).Property(x => x.FloorPaint).IsModified = true;
                break;
            case "wallpaper":
                room.PaintSettings.WallPaint = playerItem.MetaData;
                dbContext.Entry(room.PaintSettings).Property(x => x.WallPaint).IsModified = true;
                break;
            case "landscape":
                room.PaintSettings.LandscapePaint = playerItem.MetaData;
                dbContext.Entry(room.PaintSettings).Property(x => x.LandscapePaint).IsModified = true;
                break;
        }

        player.FurnitureItems.Remove(playerItem);
        await dbContext.SaveChangesAsync();
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
        {
            ItemId = ItemId
        });
        
        await room.UserRepository.BroadcastDataAsync(new RoomPaintWriter
        {
            Type = playerItem.FurnitureItem.AssetName,
            Value = playerItem.MetaData
        });
    }
}