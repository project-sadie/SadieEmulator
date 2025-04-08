using Microsoft.EntityFrameworkCore;
using Sadie.API.Networking;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserTrade
{
    List<IRoomUser> Users { get; init; }
    List<PlayerFurnitureItem> Items { get; init; }
    void OfferItems(List<PlayerFurnitureItem> playerItems);
    Task BroadcastToUsersAsync(AbstractPacketWriter writer);
    Task SwapItemsAsync(IDbContextFactory<SadieContext> dbContextFactory);
    void RemoveOfferedItem(PlayerFurnitureItem item);
}