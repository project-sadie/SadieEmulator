using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Navigator;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class SaveNavigatorSettingsEventHandler(
    SaveNavigatorSettingsEventParser eventParser,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.SaveNavigatorSettings;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        if (player?.NavigatorSettings == null)
        {
            return;
        }
        
        var navigatorSettings = player.NavigatorSettings;

        navigatorSettings.WindowX = eventParser.WindowX;
        navigatorSettings.WindowY = eventParser.WindowY;
        navigatorSettings.WindowWidth = eventParser.WindowWidth;
        navigatorSettings.WindowHeight = eventParser.WindowHeight;
        navigatorSettings.OpenSearches = eventParser.OpenSearches;

        dbContext.PlayerNavigatorSettings.Update(player.NavigatorSettings);
        await dbContext.SaveChangesAsync();
    }
}