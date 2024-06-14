using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
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
    RoomRepository roomRepository,
    PlayerRepository playerRepository,
    RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public int Category { get; set; }
    public int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var player = client.Player;
        var itemId = ItemId;
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        var roomFurnitureItem = room?.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        if (roomFurnitureItem.PlayerFurnitureItem.PlayerId != player.Id)
        {
            return;
        }

        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnPickUpAsync(room, roomFurnitureItem, client.RoomUser);
        }
        
        if (roomFurnitureItem.FurnitureItem.Type == FurnitureItemType.Floor)
        {
            await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemRemovedWriter
            {
                Id = roomFurnitureItem.Id.ToString(),
                Expired = false,
                OwnerId = roomFurnitureItem.PlayerFurnitureItem.PlayerId,
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

        var ownsItem = roomFurnitureItem.PlayerFurnitureItem.PlayerId == player.Id;
        var playerItem = roomFurnitureItem.PlayerFurnitureItem;

        room.FurnitureItems.Remove(roomFurnitureItem);
        playerItem.PlacementData.Remove(roomFurnitureItem);
        
        dbContext.Entry(roomFurnitureItem).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
        
        if (ownsItem)
        {
            await client.WriteToStreamAsync(new PlayerInventoryAddItemsWriter
            {
                FurnitureItems = [playerItem]
            });
            
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }
        else
        {
            var ownerOnline = playerRepository.GetPlayerLogicById(roomFurnitureItem.PlayerFurnitureItem.PlayerId);

            if (ownerOnline != null)
            {
                await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryAddItemsWriter
                {
                    FurnitureItems = [playerItem]
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
    