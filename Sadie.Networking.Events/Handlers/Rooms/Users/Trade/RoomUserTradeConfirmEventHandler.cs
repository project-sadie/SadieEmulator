using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.Trading;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeConfirm)]
public class RoomUserTradeConfirmEventHandler(RoomRepository roomRepository, SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
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
            UserId = roomUser.Id,
            Status = roomUser.TradeStatus
        });

        if (roomUser.Trade.Users.All(x => x.TradeStatus == 2))
        {
            await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeCloseWindowWriter());
            await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeCompletedWriter());
            await roomUser.Trade.SwapItemsAsync(dbContext);
            
            foreach (var user in roomUser.Trade.Users)
            {
                user.Trade = null;
                user.TradeStatus = 0;
            }
        }
    }
}