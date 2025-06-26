using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.SaveNavigatorSettings)]
public class SaveNavigatorSettingsEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
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
        navigatorSettings.ResultsMode = 0;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerNavigatorSettings.Update(player.NavigatorSettings);
        await dbContext.SaveChangesAsync();
    }
}