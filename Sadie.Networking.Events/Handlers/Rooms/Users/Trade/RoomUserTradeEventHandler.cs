using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerIds.RoomUserTrade)]
public class RoomUserTradeEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int TargetUserId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (roomUser.Id == TargetUserId || !room.UserRepository.TryGetById(TargetUserId, out var targetUser))
        {
            return;
        }

        if ((room.Settings.TradeOption == RoomTradeOption.RequiresRights && !roomUser.HasRights()) || 
            room.Settings.TradeOption != RoomTradeOption.Allowed)
        {
            await client.WriteToStreamAsync(new RoomUserTradeErrorWriter { Code = RoomUserTradeError.RoomTradingNotAllowed });
            return;
        }

        if (roomUser.Trade != null)
        {
            await client.WriteToStreamAsync(new RoomUserTradeErrorWriter { Code = RoomUserTradeError.SelfAlreadyTrading });
            return;
        }

        if (targetUser.Trade != null)
        {
            await client.WriteToStreamAsync(new RoomUserTradeErrorWriter { Code = RoomUserTradeError.TargetAlreadyTrading });
            return;
        }
        
        await roomUser.NetworkObject.WriteToStreamAsync(writer: new RoomUserTradeStartedWriter
        {
            UserIds = [roomUser.Id, targetUser.Id],
            State = 1
        });
        
        await targetUser.NetworkObject.WriteToStreamAsync(new RoomUserTradeStartedWriter
        {
            UserIds = [roomUser.Id, targetUser.Id],
            State = 1
        });

        var trade = new RoomUserTrade
        {
            Users = [roomUser, targetUser],
            Items = []
        };

        roomUser.Trade = trade;
        targetUser.Trade = trade;
    }
}