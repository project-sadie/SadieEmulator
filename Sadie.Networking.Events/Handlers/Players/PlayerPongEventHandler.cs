using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerPong)]
public class PlayerPongEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        client.LastPing = DateTime.Now;
        await client.WriteToStreamAsync(new PlayerPingWriter());
    }
}