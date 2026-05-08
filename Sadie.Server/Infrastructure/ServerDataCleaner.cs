using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Server;
using Sadie.Db;

namespace Sadie.Server.Infrastructure;

public class ServerDataCleaner(
    ILogger<ServerDataCleaner> logger,
    IDbContextFactory<SadieDbContext> factory)
    : IServerDataCleaner
{
    public async Task CleanAsync(CancellationToken token)
    {
        await using var context = await factory.CreateDbContextAsync(token);

        logger.LogInformation("Cleaning up data...");

        await context.PlayerData
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsOnline, false), token);
    }
}