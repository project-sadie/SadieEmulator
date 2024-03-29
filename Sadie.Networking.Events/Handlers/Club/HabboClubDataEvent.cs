using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

public class HabboClubDataEvent(HabboClubDataParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        await client.WriteToStreamAsync(new HabboClubDataWriter(parser.WindowId).GetAllBytes());
    }
}