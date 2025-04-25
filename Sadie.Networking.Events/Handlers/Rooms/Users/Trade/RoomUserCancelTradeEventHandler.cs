using Sadie.API.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(ServerPacketId.RoomUserTradeCancel)]
public class RoomUserCancelTradeEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
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
        
        foreach (var user in roomUser.Trade.Users)
        {
            user.Trade = null;
            user.TradeStatus = 0;
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
    }
}