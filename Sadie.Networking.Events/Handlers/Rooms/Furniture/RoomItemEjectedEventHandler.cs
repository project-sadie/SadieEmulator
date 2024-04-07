using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
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
    PlayerRepository playerRepository) : INetworkPacketEventHandler
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
            var currentTile = room.TileMap.FindTile(
                roomFurnitureItem.PositionX, roomFurnitureItem.PositionY);

            if (currentTile != null)
            {
                currentTile.Items.Remove(roomFurnitureItem);
                RoomHelpers.UpdateTileMapForTile(currentTile, room.TileMap);
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

        var playerItem = new PlayerFurnitureItem
        {
            FurnitureItem = roomFurnitureItem.FurnitureItem,
            LimitedData = roomFurnitureItem.LimitedData,
            MetaData = roomFurnitureItem.MetaData,
            CreatedAt = created
        };

        room.FurnitureItems.Remove(roomFurnitureItem);
        
        if (ownsItem)
        {
            player.Data.FurnitureItems.Add(playerItem);
            
            await client.WriteToStreamAsync(new PlayerInventoryAddItemsWriter([playerItem]).GetAllBytes());
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
        }
        else if (playerRepository.TryGetPlayerById(roomFurnitureItem.OwnerId, out var owner) && owner != null)
        {
            owner.Data.FurnitureItems.Remove(playerItem);
            
            await owner.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter([playerItem]).GetAllBytes());
            await owner.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
        }
        
        await dbContext.SaveChangesAsync();
    }
}
    