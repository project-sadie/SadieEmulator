using Sadie.Game.Players.Navigator;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Navigator;

public class SaveNavigatorSettingsEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;

        if (player == null)
        {
            return Task.CompletedTask;
        }
        
        var navigatorSettings = player.Data.NavigatorSettings;

        navigatorSettings.WindowX = reader.ReadInt();
        navigatorSettings.WindowY = reader.ReadInt();
        navigatorSettings.WindowWidth = reader.ReadInt();
        navigatorSettings.WindowHeight = reader.ReadInt();
        navigatorSettings.OpenSearches = reader.ReadBool();
        navigatorSettings.Unknown1 = reader.ReadInt();

        return Task.CompletedTask;
    }
}