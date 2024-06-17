using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(ServerPacketId.RoomUserTradeCancel)]
public class RoomUserCancelTradeEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
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
            UserId = roomUser.Id,
            Reason = RoomUserTradeCloseReason.Cancelled
        });
    }
}