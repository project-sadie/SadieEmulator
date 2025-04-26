using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization.Attributes;

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
        var items = new List<PlayerFurnitureItem>();
        
        foreach (var id in Ids)
        {
            var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == id);

            if (playerItem == null)
            {
                return;
            }

            items.Add(playerItem);
        }

        roomUser.Trade.OfferItems(items);
    }
}