using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Navigator;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class SaveNavigatorSettingsEvent(SaveNavigatorSettingsParser parser) : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var player = client.Player;

        if (player == null)
        {
            return Task.CompletedTask;
        }
        
        var navigatorSettings = player.Data.NavigatorSettings;

        navigatorSettings.WindowX = parser.WindowX;
        navigatorSettings.WindowY = parser.WindowY;
        navigatorSettings.WindowWidth = parser.WindowWidth;
        navigatorSettings.WindowHeight = parser.WindowHeight;
        navigatorSettings.OpenSearches = parser.OpenSearches;
        navigatorSettings.Mode = parser.Mode;

        return Task.CompletedTask;
    }
}