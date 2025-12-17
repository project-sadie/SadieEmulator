using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

public class RoomUserTradeUndoAcceptEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!RoomContextResolver.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
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
            UserId = roomUser.Player.Player.Id,
            Status = roomUser.TradeStatus
        });
    }
}