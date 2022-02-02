using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players;

namespace Sadie.Networking.Packets.Client.Unknown;

public class UnknownEvent1 : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerIgnoredUsersWriter().GetAllBytes());
    }
}