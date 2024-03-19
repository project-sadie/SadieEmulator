using Sadie.Game.Players;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Rooms.Furniture;

public class RoomFurnitureItemEjectedEvent(IRoomRepository roomRepository, 
    IPlayerInventoryDao playerInventoryDao,
    IRoomFurnitureItemDao roomFurnitureItemDao,
    IPlayerRepository playerRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var player = client.Player;
        var category = reader.ReadInteger();
        var itemId = reader.ReadInteger();
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null)
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            return;
        }

        var roomFurnitureItem = room.FurnitureItemRepository.Items.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var ownsItem = roomFurnitureItem.OwnerId == player.Data.Id;
        var created = DateTime.Now;
        
        var playerItem = new PlayerInventoryFurnitureItem(0, roomFurnitureItem.FurnitureItem,
            roomFurnitureItem.LimitedData, roomFurnitureItem.MetaData, created);

        playerItem.Id = await playerInventoryDao.CreateItemAsync(roomFurnitureItem.OwnerId, playerItem);

        await roomFurnitureItemDao.DeleteItemsAsync([roomFurnitureItem.Id]);
        room.FurnitureItemRepository.RemoveItems([roomFurnitureItem.Id]);
        
        await room.UserRepository.BroadcastDataAsync(new RoomFurnitureItemRemovedWriter(roomFurnitureItem).GetAllBytes());

        if (ownsItem)
        {
            player.Data.Inventory.AddItems([playerItem]);
            
            await client.WriteToStreamAsync(new PlayerInventoryAddItemsWriter([playerItem]).GetAllBytes());
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
        }
        else if (playerRepository.TryGetPlayerById(roomFurnitureItem.OwnerId, out var owner) && owner != null)
        {
            owner.Data.Inventory.AddItems([playerItem]);
            
            await owner.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter([playerItem]).GetAllBytes());
            await owner.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
        }
    }
}
    