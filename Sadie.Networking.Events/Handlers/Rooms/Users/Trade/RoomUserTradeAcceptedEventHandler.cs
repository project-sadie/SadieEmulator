using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeAccepted)]
public class RoomUserTradeAcceptedEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
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

        roomUser.TradeStatus = 1;
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeStatusWriter
        {
            UserId = roomUser.Player.Id,
            Status = roomUser.TradeStatus
        });

        if (roomUser.Trade.Users.All(x => x.TradeStatus == 1))
        {
            await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeWaitingConfirmWriter());
        }
    }
}