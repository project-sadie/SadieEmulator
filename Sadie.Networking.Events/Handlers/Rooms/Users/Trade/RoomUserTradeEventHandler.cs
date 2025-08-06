using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTrade)]
public class RoomUserTradeEventHandler(
    IRoomRepository roomRepository,
    IPlayerHelperService playerHelperService) : INetworkPacketEventHandler
{
    public required int TargetUserId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (roomUser.Player.Id == TargetUserId || !room.UserRepository.TryGetById(TargetUserId, out var targetUser))
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
            UserIds = [roomUser.Player.Id, targetUser.Player.Id],
            State = 1
        });
        
        await targetUser.NetworkObject.WriteToStreamAsync(new RoomUserTradeStartedWriter
        {
            UserIds = [roomUser.Player.Id, targetUser.Player.Id],
            State = 1
        });

        var trade = new RoomUserTrade(playerHelperService)
        {
            Users = [roomUser, targetUser],
            Items = []
        };

        roomUser.Trade = trade;
        targetUser.Trade = trade;
    }
}