using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomPaintItemPlaced)]
public class RoomPaintItemPlacedEventHandler(
    IRoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
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
        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
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