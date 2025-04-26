using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeConfirm)]
public class RoomUserTradeConfirmEventHandler(IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
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

        roomUser.TradeStatus = 2;
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeStatusWriter
        {
            UserId = roomUser.Player.Id,
            Status = roomUser.TradeStatus
        });

        if (roomUser.Trade.Users.All(x => x.TradeStatus == 2))
        {
            await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeCloseWindowWriter());
            await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeCompletedWriter());
            await roomUser.Trade.SwapItemsAsync(dbContextFactory);
            
            foreach (var user in roomUser.Trade.Users)
            {
                user.Trade = null;
                user.TradeStatus = 0;
            }
        }
    }
}