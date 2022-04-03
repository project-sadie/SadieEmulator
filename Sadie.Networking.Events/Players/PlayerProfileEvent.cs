using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Players;

public class PlayerProfileEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInt();
        var isThemselves = reader.ReadBool();
        
        await client.WriteToStreamAsync(new PlayerProfileWriter(client.Player).GetAllBytes());
    }
}