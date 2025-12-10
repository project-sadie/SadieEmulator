using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Server;
using Sadie.Db;

namespace Sadie.Server.Infrastructure
{
    public class ServerMigrator(
        ILogger<ServerMigrator> logger,
        IDbContextFactory<SadieMigrationsDbContext> factory)
        : IServerMigrator
    {
        public async Task MigrateAsync(CancellationToken token)
        {
            await using var context = await factory.CreateDbContextAsync(token);

            var applied = await context.Database.GetAppliedMigrationsAsync(token);
            var hasInitialMigration = applied.Any(m => m.Contains("InitialCreate"));

            if (!hasInitialMigration)
            {
                try
                {
                    logger.LogWarning("Initial database not detected. Running migrations...");
                    await context.Database.MigrateAsync(token);

                    logger.LogWarning("Seeding initial data...");
                    await SeedData.SeedInitialDataAsync(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while running migrations.");
                    throw;
                }
            }
        }
    }
}