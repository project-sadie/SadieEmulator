using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Enums.Game.Rooms.Users.Trading;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTrade)]
public class RoomUserTradeEventHandler(
    IRoomRepository roomRepository,
    IPlayerHelperService playerHelperService,
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
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

        if ((room.Room.Settings.TradeOption == RoomTradeOption.RequiresRights && !roomUser.HasRights()) || 
            room.Room.Settings.TradeOption != RoomTradeOption.Allowed)
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

        var trade = new RoomUserTrade(playerHelperService, dbContextFactory)
        {
            Users = [roomUser, targetUser],
            Items = []
        };

        roomUser.Trade = trade;
        targetUser.Trade = trade;
    }
}