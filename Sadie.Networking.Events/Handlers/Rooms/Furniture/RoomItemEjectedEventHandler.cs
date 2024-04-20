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
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        var roomFurnitureItem = room?.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        if (roomFurnitureItem.OwnerId != player.Id)
        {
            return;
        }

        if (roomFurnitureItem.FurnitureItem.Type == FurnitureItemType.Floor)
        {
            await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemRemovedWriter(
                roomFurnitureItem.Id, 
                false, 
                roomFurnitureItem.OwnerId, 
                0));
        }
        else
        {
            await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemRemovedWriter(roomFurnitureItem));
        }

        var ownsItem = roomFurnitureItem.OwnerId == player.Id;
        var created = DateTime.Now;

        var playerItem = new PlayerFurnitureItem
        {
            FurnitureItem = roomFurnitureItem.FurnitureItem,
            LimitedData = roomFurnitureItem.LimitedData,
            MetaData = roomFurnitureItem.MetaData,
            CreatedAt = created
        };

        room.FurnitureItems.Remove(roomFurnitureItem);

        dbContext.RoomFurnitureItems.Remove(roomFurnitureItem);
        await dbContext.SaveChangesAsync();
        
        if (ownsItem)
        {
            player.FurnitureItems.Add(playerItem);
            
            await client.WriteToStreamAsync(new PlayerInventoryAddItemsWriter([playerItem]));
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }
        else if (playerRepository.TryGetPlayerById(roomFurnitureItem.OwnerId, out var owner) && owner != null)
        {
            owner.FurnitureItems.Remove(playerItem);
            
            await owner.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter([playerItem]));
            await owner.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }
        
        await dbContext.SaveChangesAsync();
    }
}
    