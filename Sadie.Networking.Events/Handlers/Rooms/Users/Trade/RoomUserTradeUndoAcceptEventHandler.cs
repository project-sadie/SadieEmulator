using Sadie.API.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Networking.Client;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

public class RoomUserTradeUndoAcceptEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return;
        }

        if (roomUser.Trade == null || roomUser.TradeStatus < 1)
        {
            return;
        }

        roomUser.TradeStatus = 0;
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeStatusWriter
        {
            UserId = roomUser.Player.Id,
            Status = roomUser.TradeStatus
        });
    }
}