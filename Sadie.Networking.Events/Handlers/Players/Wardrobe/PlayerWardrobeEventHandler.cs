using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Wardrobe;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

[PacketId(EventHandlerId.PlayerWardrobe)]
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