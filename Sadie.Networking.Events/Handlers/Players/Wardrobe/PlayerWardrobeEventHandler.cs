using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Wardrobe;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

[PacketId(EventHandlerIds.PlayerWardrobe)]
public class PlayerWardrobeEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerWardrobeWriter
        {
            State = 1,
            Outfits = client.Player.WardrobeItems
        });
    }
}