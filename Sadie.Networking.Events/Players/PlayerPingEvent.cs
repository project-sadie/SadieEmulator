using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Players;

public class PlayerPingEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        client.LastPing = DateTime.Now;
        
        await client.WriteToStreamAsync(new PlayerPongWriter(reader.ReadInteger()).GetAllBytes());
    }
}