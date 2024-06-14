using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerPing)]
public class PlayerPingEventHandler : INetworkPacketEventHandler
{
    public int Id { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new PlayerPongWriter
        {
            Id = Id
        });
    }
}