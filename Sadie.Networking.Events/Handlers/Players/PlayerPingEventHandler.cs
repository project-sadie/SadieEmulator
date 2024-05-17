using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerPing)]
public class PlayerPingEventHandler(PlayerPingEventParser eventParser) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
     
        await client.WriteToStreamAsync(new PlayerPongWriter
        {
            Id = eventParser.Id
        });
    }
}