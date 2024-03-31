using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerActivityEventHandler(PlayerActivityEventParser eventParser) : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient networkClient, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        return Task.CompletedTask;
    }
}