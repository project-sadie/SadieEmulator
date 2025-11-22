using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.SaveNavigatorSettings)]
public class SaveNavigatorSettingsEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper,
    ILogger<SaveNavigatorSettingsEventHandler> logger) : INetworkPacketEventHandler
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
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var entity = await dbContext.PlayerNavigatorSettings
            .FirstOrDefaultAsync(x => x.PlayerId == player.Id);

        if (entity == null)
        {
            #pragma warning disable CA1873
            logger.LogWarning("PlayerNavigatorSettings missing for player {PlayerId}", player.Id);
            #pragma warning restore CA1873
            return;
        }

        mapper.Map(navigatorSettings, entity);
        await dbContext.SaveChangesAsync();
    }
}