using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database;
using Sadie.Enums.Game.Furniture;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemEjected)]
public class RoomItemEjectedEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomRepository roomRepository,
    IPlayerRepository playerRepository,
    IRoomFurnitureItemInteractorRepository interactorRepository,
    IRoomTileMapHelperService tileMapHelperService) : INetworkPacketEventHandler
{
    public int Category { get; init; }
    public int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var player = client.Player;
        var itemId = ItemId;
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        var roomFurnitureItem = room?
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }
        
        var ownsItem = roomFurnitureItem.PlayerFurnitureItem.PlayerId == player.Id;
        
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
                Id = roomFurnitureItem.PlayerFurnitureItemId.ToString(),
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
        
        var point = new Point(
            roomFurnitureItem.PositionX,
            roomFurnitureItem.PositionY);
        
        foreach (var user in tileMapHelperService.GetUsersAtPoints([point], room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }

        var itemRecord = roomFurnitureItem.PlayerFurnitureItem;
        
        if (ownsItem)
        {
            itemRecord = client.Player.FurnitureItems.FirstOrDefault(x => x.Id == roomFurnitureItem.PlayerFurnitureItemId);

            if (itemRecord == null)
            {
                return;
            }
            
            await client.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
            {
                Count = 1,
                Category = 1,
                FurnitureItems = [itemRecord]
            });
            
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }
        else
        {
            var ownerOnline = playerRepository.GetPlayerLogicById(roomFurnitureItem.PlayerFurnitureItem.PlayerId);

            if (ownerOnline is { NetworkObject: not null })
            {
                itemRecord = ownerOnline.FurnitureItems.FirstOrDefault(x => x.Id == roomFurnitureItem.PlayerFurnitureItemId);

                if (itemRecord == null)
                {
                    return;
                }

                await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
                {
                    Count = 1,
                    Category = 1,
                    FurnitureItems = [itemRecord]
                });
                
                await ownerOnline.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
                
                ownerOnline.FurnitureItems.Add(itemRecord);
            }
        }
        
        itemRecord.PlacementData = null;
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
    }
}
    