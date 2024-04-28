using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Wardrobe;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

public class PlayerWardrobeEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerWardrobe;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
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