using Sadie.Game.Players;
using Sadie.Game.Players.Navigator;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Navigator;

public class SaveNavigatorSettingsEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;

        if (player != null)
        {
            player.NavigatorSettings = new PlayerNavigatorSettings(
                reader.ReadInt(),
                reader.ReadInt(),
                reader.ReadInt(),
                reader.ReadInt(),
                reader.ReadBool(),
                reader.ReadInt());
        }

        return Task.CompletedTask;
    }
}