using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerIds.SaveNavigatorSettings)]
public class SaveNavigatorSettingsEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool OpenSearches { get; set; }
    public int Mode { get; set; }

    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player?.NavigatorSettings == null)
        {
            return;
        }
        
        var navigatorSettings = player.NavigatorSettings;

        navigatorSettings.WindowX = WindowX;
        navigatorSettings.WindowY = WindowY;
        navigatorSettings.WindowWidth = WindowWidth;
        navigatorSettings.WindowHeight = WindowHeight;
        navigatorSettings.OpenSearches = OpenSearches;

        dbContext.PlayerNavigatorSettings.Update(player.NavigatorSettings);
        await dbContext.SaveChangesAsync();
    }
}