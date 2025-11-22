using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Writers.Players.Wardrobe;
using Sadie.Shared.Attributes;

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