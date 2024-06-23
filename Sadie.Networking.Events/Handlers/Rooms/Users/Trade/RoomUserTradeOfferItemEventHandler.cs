using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeOfferItem)]
public class RoomUserTradeOfferItemEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    
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

        var player = client.Player;
        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            return;
        }

        roomUser.Trade.OfferItems(roomUser, [playerItem]);
    }
}