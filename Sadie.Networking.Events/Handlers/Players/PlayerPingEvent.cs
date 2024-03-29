using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerPingEvent(PlayerPingParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        await client.WriteToStreamAsync(new PlayerPongWriter(parser.Id).GetAllBytes());
    }
}