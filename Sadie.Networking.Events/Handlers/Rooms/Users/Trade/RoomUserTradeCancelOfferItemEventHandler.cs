using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Networking.Client;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

public class RoomUserTradeCancelOfferItemEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return;
        }

        if (roomUser.Trade == null || roomUser.TradeStatus > 0)
        {
            return;
        }

        var player = client.Player;
        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            return;
        }

        foreach (var user in roomUser.Trade.Users)
        {
            user.TradeStatus = 0;
        }
        
        roomUser.Trade.RemoveOfferedItem(playerItem);
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeUpdateWriter
        {
            Trade = roomUser.Trade
        });
    }
}