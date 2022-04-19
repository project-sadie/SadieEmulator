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

        navigatorSettings.WindowX = reader.ReadInteger();
        navigatorSettings.WindowY = reader.ReadInteger();
        navigatorSettings.WindowWidth = reader.ReadInteger();
        navigatorSettings.WindowHeight = reader.ReadInteger();
        navigatorSettings.OpenSearches = reader.ReadBool();
        navigatorSettings.Unknown1 = reader.ReadInteger();

        return Task.CompletedTask;
    }
}