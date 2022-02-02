using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerSanctionStatusEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerSanctionStatusWriter().GetAllBytes());
    }
}