using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerActivityEvent(PlayerActivityParser parser) : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient networkClient, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        return Task.CompletedTask;
    }
}