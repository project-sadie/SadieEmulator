using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Packets.Writers.Rooms.Users.Trading;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

public class RoomUserTradeCancelOfferItemEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return;
        }

        if (roomUser.Trade == null || roomUser.TradeStatus > 0)
        {
            return;
        }

        var player = client.Player;
        var playerItem = player.Player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            return;
        }

        foreach (var user in roomUser.Trade.Users)
        {
            user.TradeStatus = 0;
        }
        
        roomUser.Trade.RemoveOfferedItem(playerItem);
        
        await roomUser.Trade.BroadcastToUsersAsync(new RoomUserTradeUpdateWriter
        {
            Trade = roomUser.Trade
        });
    }
}