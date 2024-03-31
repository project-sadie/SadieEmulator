using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Wardrobe;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

public class PlayerWardrobeEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }

        var wardrobeItems = client.Player.Data.WardrobeComponent.WardrobeItems;
        await client.WriteToStreamAsync(new PlayerWardrobeWriter(1, wardrobeItems).GetAllBytes());
    }
}