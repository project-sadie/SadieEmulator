using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Networking;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events;

public class RoomUserTrade(IPlayerHelperService playerHelperService) : IRoomUserTrade
{
    public required List<IRoomUser> Users { get; init; }
    public required List<PlayerFurnitureItem> Items { get; init; }
    
    public async void OfferItems(List<PlayerFurnitureItem> playerItems)
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
        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        
        foreach (var roomUser in Users)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(serializedObject);
        }
    }
    
    public async Task SwapItemsAsync(IDbContextFactory<SadieContext> dbContextFactory)
    {
        var map = new Dictionary<long, List<PlayerFurnitureItem>>();
        
        foreach (var item in Items)
        {
            if (!map.ContainsKey(item.PlayerId))
            {
                map[item.PlayerId] = [];
            }

            map[item.PlayerId].Add(item);
            
        }

        var userOne = Users[0].Player;
        var userTwo = Users[1].Player;

        var userOneItems = map.TryGetValue(userOne.Id, out var oneItems) ? 
            oneItems : [];
        
        var userTwoItems = map.TryGetValue(userTwo.Id, out var twoItems) ? 
            twoItems : [];
        
        var updateMap = new Dictionary<IPlayerLogic, List<PlayerFurnitureItem>>();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        foreach (var userOneItem in userOneItems)
        {
            userOneItem.PlayerId = userTwo.Id;
            userOneItem.Player = (Player) userTwo;

            userOne.FurnitureItems.Remove(userOneItem);
            userTwo.FurnitureItems.Add(userOneItem);

            if (!updateMap.ContainsKey(userTwo))
            {
                updateMap[userTwo] = [];
            }

            updateMap[userTwo].Add(userOneItem);
            
            dbContext.Entry(userOneItem).State = EntityState.Modified;
        }
        
        foreach (var userTwoItem in userTwoItems)
        {
            userTwoItem.PlayerId = userOne.Id;
            userTwoItem.Player = (Player) userOne;

            userTwo.FurnitureItems.Remove(userTwoItem);
            userOne.FurnitureItems.Add(userTwoItem);

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

    public void RemoveOfferedItem(PlayerFurnitureItem item)
    {
        Items.Remove(item);
    }
}