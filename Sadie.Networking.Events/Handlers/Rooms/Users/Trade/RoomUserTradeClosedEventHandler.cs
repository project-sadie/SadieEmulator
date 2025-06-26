using Sadie.API.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeClosed)]
public class RoomUserTradeClosedEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return;
        }

        if (roomUser.Trade == null)
        {
            return;
        }

        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeUpdateWriter
        {
            Trade = roomUser.Trade
        });
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeClosedWriter
        {
            UserId = roomUser.Player.Id,
            Reason = RoomUserTradeCloseReason.Cancelled
        });

        foreach (var user in roomUser.Trade.Users)
        {
            user.Trade = null;
            user.TradeStatus = 0;
        }
    }
}