using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Players;

public class PlayerSanctionStatusEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerSanctionStatusWriter(
            false,
            false, 
            "ALERT", 
            0, 
            30,
            "cfh.reason.EMPTY", 
            DateTime.Now, 
            0,
            "ALERT",
            0,
            30,
            false, 
            DateTime.MinValue).GetAllBytes());
    }
}