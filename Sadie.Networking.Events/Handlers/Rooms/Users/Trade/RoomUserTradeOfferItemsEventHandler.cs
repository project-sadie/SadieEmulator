using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeOfferItems)]
public class RoomUserTradeOfferItemsEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public List<int> Ids { get; init; } = [];
    
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
        var items = new List<PlayerFurnitureItemDto>();
        
        foreach (var id in Ids)
        {
            var playerItem = player.Player.FurnitureItems.FirstOrDefault(x => x.Id == id);

            if (playerItem == null)
            {
                return;
            }

            items.Add(playerItem);
        }

        await roomUser.Trade.OfferItemsAsync(items);
    }
}