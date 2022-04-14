using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Club;

public class HabboClubDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        client.WriteToStreamAsync(new HabboClubDataWriter(reader.ReadInteger()).GetAllBytes());
    }
}