using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomPaintItemPlacedEventHandler(
    RoomPaintItemPlacedEventParser parser, 
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomPaintItemPlaced;
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

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
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        
        if (!found || room == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == parser.ItemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        switch (playerItem.FurnitureItem.AssetName)
        {
            case "floor":
                room.PaintSettings.FloorPaint = playerItem.MetaData;
                break;
            case "wallpaper":
                room.PaintSettings.WallPaint = playerItem.MetaData;
                break;
            case "landscape":
                room.PaintSettings.LandscapePaint = playerItem.MetaData;
                break;
        }

        player.FurnitureItems.Remove(playerItem);
        await dbContext.SaveChangesAsync();
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter(parser.ItemId).GetAllBytes());
        await room.UserRepository.BroadcastDataAsync(new RoomPaintWriter(playerItem.FurnitureItem.AssetName, playerItem.MetaData).GetAllBytes());
    }
}