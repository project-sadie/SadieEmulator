using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Other;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerPingEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        client.LastPing = DateTime.Now;
        
        await client.WriteToStreamAsync(new PlayerPongWriter(reader.ReadInt()).GetAllBytes());
    }
}