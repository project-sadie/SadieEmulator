using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerPingEventHandler(PlayerPingEventParser eventParser) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerPing;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        await client.WriteToStreamAsync(new PlayerPongWriter(eventParser.Id));
    }
}