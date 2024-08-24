using Sadie.API.Game.Players.Packets.Writers;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Shared.Dtos;

namespace Sadie.API.Game.Players;

public interface IPlayerHelperService
{
    Task SendFriendUpdatesToPlayerAsync(
        IPlayerLogic player, 
        List<PlayerFriendshipUpdate> updates);

    Task SendPlayerFriendListUpdate(
        IPlayerLogic player, 
        IPlayerRepository playerRepository);

    IPlayerSubscriptionWriter? GetSubscriptionWriterAsync(IPlayerLogic player, string name);

    Task UpdatePlayerStatusForFriendsAsync(
        IPlayerLogic player, 
        IEnumerable<PlayerFriendship> friendships, 
        bool isOnline, 
        bool inRoom,
        IPlayerRepository playerRepository);

    Task SendUnseenInventoryItemsAsync(IPlayerLogic player, List<PlayerFurnitureItem> items);
    
    Task RefreshInventoryAsync(IPlayerLogic player);
}