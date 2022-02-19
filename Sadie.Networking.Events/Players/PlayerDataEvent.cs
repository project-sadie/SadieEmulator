using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Players;

public class PlayerDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerData = (Game.Players.PlayerData) client.Player!;

        await client.WriteToStreamAsync(new PlayerDataWriter(playerData).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerPerksWriter().GetAllBytes());
    }
}