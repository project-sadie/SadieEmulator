using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.Database;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemEjected)]
public class RoomItemEjectedEventHandler(
    SadieContext dbContext,
    IRoomRepository roomRepository,
    IPlayerRepository playerRepository,
    IRoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public int Category { get; set; }
    public int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var player = client.Player;
        var itemId = ItemId;
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        var roomFurnitureItem = room?.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        if (roomFurnitureItem.PlayerFurnitureItem.PlayerId != player.Id)
        {
            return;
        }

        var ownsItem = roomFurnitureItem.PlayerFurnitureItem.PlayerId == player.Id;
        var playerItem = client.Player.FurnitureItems.FirstOrDefault(x => x.Id == roomFurnitureItem.Id);

        if (playerItem == null)
        {
            return;
        }
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

        foreach (var interactor in interactors)
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

        room.FurnitureItems.Remove(roomFurnitureItem);
        playerItem.PlacementData = null;
        
        dbContext.Entry(roomFurnitureItem).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
        
        if (ownsItem)
        {
            await client.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
            {
                Count = 1,
                Category = 1,
                FurnitureItems = [playerItem]
            });
            
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }
        else
        {
            var ownerOnline = playerRepository.GetPlayerLogicById(roomFurnitureItem.PlayerFurnitureItem.PlayerId);

            if (ownerOnline != null)
            {
                await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
                {
                    Count = 1,
                    Category = 1,
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
    