using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Other;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerPingEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerPongWriter(reader.ReadInt()).GetAllBytes());
    }
}