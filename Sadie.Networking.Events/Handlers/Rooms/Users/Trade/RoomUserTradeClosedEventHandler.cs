using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Users.Trading;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Rooms.Users.Trading;

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
            UserId = roomUser.Player.Player.Id,
            Reason = RoomUserTradeCloseReason.Cancelled
        });

        foreach (var user in roomUser.Trade.Users)
        {
            user.Trade = null;
            user.TradeStatus = 0;
        }
    }
}