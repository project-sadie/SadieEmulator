using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerActivity)]
public class PlayerActivityEventHandler(
    PlayerActivityEventParser eventParser) : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient networkClient, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        return Task.CompletedTask;
    }
}