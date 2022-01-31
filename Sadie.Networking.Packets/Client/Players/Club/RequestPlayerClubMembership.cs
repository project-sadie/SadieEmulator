using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Other;

namespace Sadie.Networking.Packets.Client.Players.Club;

public class RequestPlayerClubMembership : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerClub(reader.ReadString()).GetAllBytes());
    }
}