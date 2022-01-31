using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Other;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerData = (Game.Players.PlayerData) client.Player;

        await client.WriteToStreamAsync(new PlayerData(playerData).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerPerks().GetAllBytes());
    }
}