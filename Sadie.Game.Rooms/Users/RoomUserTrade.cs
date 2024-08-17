using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Players.Packets.Writers;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Users;

public class RoomUserTrade : IRoomUserTrade
{
    public required List<IRoomUser> Users { get; init; }
    public required List<PlayerFurnitureItem> Items { get; init; }
    
    public async void OfferItems(IRoomUser roomUser, List<PlayerFurnitureItem> playerItems)
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
    
    public async Task SwapItemsAsync(SadieContext dbContext)
    {
        var map = new Dictionary<int, List<PlayerFurnitureItem>>();
        
        foreach (var item in Items)
        {
            if (!map.ContainsKey(item.PlayerId))
            {
                map[item.PlayerId] = [];
            }

            map[item.PlayerId].Add(item);
            
        }

        var userOne = Users[0];
        var userTwo = Users[1];

        var userOneItems = map.TryGetValue(userOne.Id, out var oneItems) ? 
            oneItems : [];
        
        var userTwoItems = map.TryGetValue(userTwo.Id, out var twoItems) ? 
            twoItems : [];
        
        var updateMap = new Dictionary<IRoomUser, List<PlayerFurnitureItem>>();

        foreach (var userOneItem in userOneItems)
        {
            userOneItem.PlayerId = userTwo.Id;
            userOneItem.Player = (Player) userTwo.Player;

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
            userTwoItem.PlayerId = userOne.Id;
            userTwoItem.Player = (Player) userOne.Player;

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
            await updatePlayer.NetworkObject.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
            {
                Count = updatedItems.Count,
                Category = 1,
                FurnitureItems = updatedItems
            });
                
            await updatePlayer.NetworkObject.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
        }

        await dbContext.SaveChangesAsync();
    }

    public void RemoveOfferedItem(PlayerFurnitureItem item)
    {
        Items.Remove(item);
    }
}