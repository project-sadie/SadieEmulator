using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Navigator;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class SaveNavigatorSettingsEventHandler(SaveNavigatorSettingsEventParser eventParser) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.SaveNavigatorSettings;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        if (player == null)
        {
            return Task.CompletedTask;
        }
        
        var navigatorSettings = player.NavigatorSettings;

        navigatorSettings.WindowX = eventParser.WindowX;
        navigatorSettings.WindowY = eventParser.WindowY;
        navigatorSettings.WindowWidth = eventParser.WindowWidth;
        navigatorSettings.WindowHeight = eventParser.WindowHeight;
        navigatorSettings.OpenSearches = eventParser.OpenSearches;
        navigatorSettings.Mode = eventParser.Mode;

        return Task.CompletedTask;
    }
}