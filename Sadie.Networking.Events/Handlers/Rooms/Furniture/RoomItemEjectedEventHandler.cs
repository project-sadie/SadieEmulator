using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomFurnitureItemEjected)]
public class RoomItemEjectedEventHandler(
    SadieContext dbContext,
    RoomFurnitureItemEjectedEventParser eventParser,
    RoomRepository roomRepository,
    PlayerRepository playerRepository) : INetworkPacketEventHandler
{
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
            await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemRemovedWriter
            {
                Id = roomFurnitureItem.Id.ToString(),
                Expired = false,
                OwnerId = roomFurnitureItem.OwnerId,
                Delay = 0
            });
        }
        else
        {
            await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemRemovedWriter
            {
                Item = roomFurnitureItem
            });
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
            
            await client.WriteToStreamAsync(new PlayerInventoryAddItemsWriter
            {
                Items = [playerItem]
            });
            
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }
        else
        {
            var ownerOnline = playerRepository.GetPlayerLogicById(roomFurnitureItem.OwnerId);

            if (ownerOnline != null)
            {
                await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter
                {
                    Items = [playerItem]
                });
                
                await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
                
                ownerOnline.FurnitureItems.Add(playerItem);
            }
            else
            {
                dbContext.PlayerFurnitureItems.Add(playerItem);
            }
        }
        
        await dbContext.SaveChangesAsync();
    }
}
    