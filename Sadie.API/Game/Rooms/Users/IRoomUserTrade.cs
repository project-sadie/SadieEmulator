using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserTrade
{
    List<IRoomUser> Users { get; init; }
    List<PlayerFurnitureItem> Items { get; init; }
    void OfferItems(List<PlayerFurnitureItem> playerItems);
    Task BroadcastToUsersAsync(AbstractPacketWriter writer);
    Task SwapItemsAsync(SadieContext dbContext);
    void RemoveOfferedItem(PlayerFurnitureItem item);
}