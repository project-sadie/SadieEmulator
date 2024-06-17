using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerIds.RoomUserTradeClosed)]
public class RoomUserTradeClosedEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
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
            UserId = roomUser.Id,
            Reason = RoomUserTradeCloseReason.Cancelled
        });

        foreach (var user in roomUser.Trade.Users)
        {
            user.Trade = null;
            user.TradeStatus = 0;
        }
    }
}