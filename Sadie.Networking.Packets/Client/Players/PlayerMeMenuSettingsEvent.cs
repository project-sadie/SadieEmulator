using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Other;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerMeMenuSettingsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerMeMenuSettingsWriter().GetAllBytes());
    }
}