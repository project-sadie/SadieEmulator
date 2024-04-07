using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomItemEjectedEventHandler(
    SadieContext dbContext,
    RoomFurnitureItemEjectedEventParser eventParser,
    RoomRepository roomRepository, 
    IPlayerInventoryDao playerInventoryDao,
    IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomFurnitureItemEjected;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var player = client.Player;
        var itemId = eventParser.ItemId;
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null)
        {
            return;
        }

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        if (roomFurnitureItem.OwnerId != player.Data.Id)
        {
            return;
        }

        if (roomFurnitureItem.FurnitureItem.Type == FurnitureItemType.Floor)
        {
            var currentTile = room.LayoutData.FindTile(
                roomFurnitureItem.PositionX, roomFurnitureItem.PositionY);

            if (currentTile != null)
            {
                currentTile.Items.Remove(roomFurnitureItem);
                RoomHelpers.UpdateTileMapForTile(currentTile, room.LayoutData);
            }
            
            await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemRemovedWriter(
                roomFurnitureItem.Id, 
                false, 
                roomFurnitureItem.OwnerId, 
                0).GetAllBytes());
        }
        else
        {
            await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemRemovedWriter(roomFurnitureItem).GetAllBytes());
        }

        var ownsItem = roomFurnitureItem.OwnerId == player.Data.Id;
        var created = DateTime.Now;
        
        var playerItem = new PlayerInventoryFurnitureItem(0, roomFurnitureItem.FurnitureItem,
            roomFurnitureItem.LimitedData, roomFurnitureItem.MetaData, created);

        playerItem.Id = await playerInventoryDao.CreateItemAsync(roomFurnitureItem.OwnerId, playerItem);

        room.FurnitureItems.Remove(roomFurnitureItem);
        await dbContext.SaveChangesAsync();
        
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
    