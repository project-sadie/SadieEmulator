using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeAccepted)]
public class RoomUserTradeAcceptedEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
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

        roomUser.TradeStatus = 1;
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeStatusWriter
        {
            UserId = roomUser.Id,
            Status = roomUser.TradeStatus
        });

        if (roomUser.Trade.Users.All(x => x.TradeStatus == 1))
        {
            await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeWaitingConfirmWriter());
        }
    }
}