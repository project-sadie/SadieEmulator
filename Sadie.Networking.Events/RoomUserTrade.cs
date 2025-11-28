using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Db;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events;

public class RoomUserTrade(
    IPlayerHelperService playerHelperService,
    IDbContextFactory<SadieDbContext> dbContextFactory) : IRoomUserTrade
{
    public required List<IRoomUser> Users { get; init; }
    public required List<PlayerFurnitureItemDto> Items { get; init; }
    
    public async void OfferItems(List<PlayerFurnitureItemDto> playerItems)
    {
        foreach (var item in playerItems.Where(item => !Items.Contains(item)))
        {
            Items.Add(item);
        }

        foreach (var user in Users)
        {
            user.TradeStatus = 0;
        }

        await BroadcastToUsersAsync(new RoomUserTradeUpdateWriter
        {
            Trade = this
        });
    }
    
    public async Task BroadcastToUsersAsync(AbstractPacketWriter writer)
    {
        var serializedObject = await NetworkPacketWriterSerializer.SerializeAsync(writer);
        
        foreach (var roomUser in Users)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(serializedObject);
        }
    }
    
    public async Task SwapItemsAsync()
    {
        var map = new Dictionary<long, List<PlayerFurnitureItemDto>>();
        
        foreach (var item in Items)
        {
            if (!map.TryGetValue(item.PlayerId, out var value))
            {
                value = [];
                map[item.PlayerId] = value;
            }

            value.Add(item);
        }

        var userOne = Users[0].Player;
        var userTwo = Users[1].Player;

        var userOneItems = map.TryGetValue(userOne.Player.Id, out var oneItems) ? 
            oneItems : [];
        
        var userTwoItems = map.TryGetValue(userTwo.Player.Id, out var twoItems) ? 
            twoItems : [];
        
        var updateMap = new Dictionary<IPlayerLogic, List<PlayerFurnitureItemDto>>();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        foreach (var userOneItem in userOneItems)
        {
            userOneItem.PlayerId = userTwo.Player.Id;

            userOne.Player.FurnitureItems.Remove(userOneItem);
            userTwo.Player.FurnitureItems.Add(userOneItem);

            if (!updateMap.ContainsKey(userTwo))
            {
                updateMap[userTwo] = [];
            }

            updateMap[userTwo].Add(userOneItem);
            
            dbContext.Entry(userOneItem).State = EntityState.Modified;
        }
        
        foreach (var userTwoItem in userTwoItems)
        {
            userTwoItem.PlayerId = userOne.Player.Id;

            userTwo.Player.FurnitureItems.Remove(userTwoItem);
            userOne.Player.FurnitureItems.Add(userTwoItem);

            if (!updateMap.ContainsKey(userOne))
            {
                updateMap[userOne] = [];
            }

            updateMap[userOne].Add(userTwoItem);
            
            dbContext.Entry(userTwoItem).State = EntityState.Modified;
        }

        foreach (var (updatePlayer, updatedItems) in updateMap)
        {
            await playerHelperService.SendUnseenInventoryItemsAsync(updatePlayer, updatedItems);
            await playerHelperService.RefreshInventoryAsync(updatePlayer);
        }

        await dbContext.SaveChangesAsync();
    }

    public void RemoveOfferedItem(PlayerFurnitureItemDto item)
    {
        Items.Remove(item);
    }
}