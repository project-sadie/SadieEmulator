using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Trade;

[PacketId(EventHandlerId.RoomUserTradeOfferItems)]
public class RoomUserTradeOfferItemsEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public List<int> Ids { get; set; } = [];
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
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

        roomUser.Trade.OfferItems(roomUser, items);
    }
}