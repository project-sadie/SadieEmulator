using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Wardrobe;

namespace Sadie.Networking.Events.Players.Wardrobe;

public class PlayerWardrobeEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }

        var wardrobeItems = client.Player.Data.WardrobeComponent.WardrobeItems;
        await client.WriteToStreamAsync(new PlayerWardrobeWriter(wardrobeItems).GetAllBytes());
    }
}