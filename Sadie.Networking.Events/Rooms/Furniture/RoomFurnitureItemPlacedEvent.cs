using Sadie.Game.Furniture;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared;
using Sadie.Shared.Game.Rooms;
using Sadie.Shared.Networking;

namespace Sadie.Networking.Events.Rooms.Furniture;

public class RoomFurnitureItemPlacedEvent(IRoomRepository roomRepository, 
    IPlayerInventoryDao playerInventoryDao,
    IRoomFurnitureItemDao roomFurnitureItemDao) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var placementData = reader.ReadString().Split(" ");
        
        if (!int.TryParse(placementData[0], out var itemId))
        {
            await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var playerItem = player.Data.Inventory.Items.FirstOrDefault(x => x.Id == itemId);

        if (playerItem == null)
        {
            await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null)
        {
            await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        RoomFurnitureItem roomFurnitureItem = null;

        if (playerItem.Item.Type == FurnitureItemType.Floor)
        {
            if (!int.TryParse(placementData[1], out var x) ||
                !int.TryParse(placementData[2], out var y) || 
                !int.TryParse(placementData[3], out var direction))
            {
                await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
                return;
            }

            var tile = room.Layout.FindTile(x, y);

            if (tile == null || tile.State == RoomTileState.Closed)
            {
                await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
                return;
            }
            
            var created = DateTime.Now;
            var z = 1; // TODO: check this
            
            roomFurnitureItem = new RoomFurnitureItem(
                0, 
                room.Id,
                player.Data.Id,
                player.Data.Username,
                playerItem.Item, 
                new HPoint(x, y, z), 
                (HDirection)direction,
                playerItem.LimitedData,
                playerItem.MetaData,
                created);
        }
        else if (playerItem.Item.Type == FurnitureItemType.Wall)
        {
            
        }

        if (roomFurnitureItem == null)
        {
            await SendErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        await playerInventoryDao.DeleteItemsAsync([itemId]);
        roomFurnitureItem.Id = await roomFurnitureItemDao.CreateItemAsync(roomFurnitureItem);

        room.FurnitureItemRepository.AddItems([roomFurnitureItem]);
        //player.Data.Inventory.RemoveItems([itemId]);
        
        //await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter(itemId).GetAllBytes());
        await room.UserRepository.BroadcastDataAsync(new RoomFurnitureItemPlacedWriter(roomFurnitureItem).GetAllBytes());
    }

    private static async Task SendErrorAsync(INetworkObject client, FurniturePlacementError error)
    {
        await client.WriteToStreamAsync(new NotificationWriter(NotificationType.FurniturePlacementError,
            new Dictionary<string, string>()
            {
                { "message", error.ToString() }
            }).GetAllBytes());
    }
}
    