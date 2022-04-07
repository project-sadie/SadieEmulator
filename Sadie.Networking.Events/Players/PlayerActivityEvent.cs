using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players;

public class PlayerActivityEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient networkClient, INetworkPacketReader reader)
    {
        if (networkClient.Player is {Authenticated: true})
        {
            return;
        }
        
        var type = reader.ReadString();
        var value = reader.ReadString();
        var action = reader.ReadString();
    }
}