using Sadie.Game.Furniture;
using Sadie.Game.Players;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Networking;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Rooms.Furniture;

public class RoomFurnitureItemPlacedEvent(
    IRoomRepository roomRepository, 
    IPlayerInventoryDao playerInventoryDao,
    IRoomFurnitureItemDao roomFurnitureItemDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var placementData = reader.ReadString().Split(" ");
        
        if (!int.TryParse(placementData[0], out var itemId))
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var playerItem = player.Data.Inventory.Items.FirstOrDefault(x => x.Id == itemId);

        if (playerItem == null)
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null)
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }

        if (playerItem.Item.Type == FurnitureItemType.Floor)
        {
            await OnFloorItemAsync(
                placementData, 
                room, 
                client, 
                player, 
                playerItem,
                itemId);
        }
        else if (playerItem.Item.Type == FurnitureItemType.Wall)
        {
            await OnWallItemAsync(placementData, room, player, playerItem, itemId, client);
        }
    }

    private async Task OnFloorItemAsync(
        string[] placementData, 
        IRoom room, 
        INetworkClient client, 
        IPlayer player, 
        PlayerInventoryFurnitureItem playerItem, 
        int itemId)
    {
        if (!int.TryParse(placementData[1], out var x) ||
            !int.TryParse(placementData[2], out var y) || 
            !int.TryParse(placementData[3], out var direction))
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var tile = room.Layout.FindTile(x, y);

        if (tile == null || tile.State == RoomTileState.Closed)
        {
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
            
        var created = DateTime.Now;
        var z = 0; // TODO: check this
            
        var roomFurnitureItem = new RoomFurnitureItem(
            0, 
            room.Id,
            player.Data.Id,
            player.Data.Username,
            playerItem.Item, 
            new HPoint(x, y, z), 
            string.Empty,
            (HDirection)direction,
            playerItem.LimitedData,
            playerItem.MetaData,
            created);

        tile.Items.Add(roomFurnitureItem);
        RoomHelpers.UpdateTileMapForTile(tile, room.Layout);
        
        await playerInventoryDao.DeleteItemsAsync([itemId]);
        roomFurnitureItem.Id = await roomFurnitureItemDao.CreateItemAsync(roomFurnitureItem);

        room.FurnitureItemRepository.AddItems([roomFurnitureItem]);
        player.Data.Inventory.RemoveItems([itemId]);
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter(itemId).GetAllBytes());
        
        await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemPlacedWriter(
            roomFurnitureItem.Id,
            roomFurnitureItem.FurnitureItem.AssetId,
            roomFurnitureItem.Position,
            (int) roomFurnitureItem.Direction,
            roomFurnitureItem.FurnitureItem.StackHeight,
            1,
            (int) ObjectDataKey.MapKey,
            new Dictionary<string, string>(),
            roomFurnitureItem.FurnitureItem.InteractionType,
            roomFurnitureItem.MetaData,
            roomFurnitureItem.FurnitureItem.InteractionModes,
            -1,
            roomFurnitureItem.OwnerId,
            roomFurnitureItem.OwnerUsername).GetAllBytes());
    }

    private async Task OnWallItemAsync(
        IReadOnlyList<string> placementData,
        IRoomData room,
        IPlayer player,
        PlayerInventoryFurnitureItem playerItem,
        int itemId,
        INetworkObject client)
    {
        var wallPosition = $"{placementData[1]} {placementData[2]} {placementData[3]}";
            
        var roomFurnitureItem = new RoomFurnitureItem(
            0, 
            room.Id,
            player.Data.Id,
            player.Data.Username,
            playerItem.Item, 
            new HPoint(0, 0, 0),
            wallPosition,
            0,
            playerItem.LimitedData,
            playerItem.MetaData,
            DateTime.Now);
        
        await playerInventoryDao.DeleteItemsAsync([itemId]);
        roomFurnitureItem.Id = await roomFurnitureItemDao.CreateItemAsync(roomFurnitureItem);
        
        room.FurnitureItemRepository.AddItems([roomFurnitureItem]);
        player.Data.Inventory.RemoveItems([itemId]);
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter(itemId).GetAllBytes());
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemPlacedWriter(roomFurnitureItem)
            .GetAllBytes());
    }
}
    