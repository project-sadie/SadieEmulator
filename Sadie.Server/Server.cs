using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Server;
using Sadie.API.Interfaces.Server.Tasks;
using IServer = Sadie.API.IServer;

namespace Sadie.Server;

public class Server(
    ILogger<Server> logger,
    IServerMigrator migrator,
    IServerDataCleaner dataCleaner,
    IServerTaskWorker taskWorker,
    INetworkClientRepository networkClientRepository,
    ICatalogPageRepository catalogPageRepository) : IServer
{
    
    public async Task RunAsync(CancellationToken token)
    {
        await migrator.MigrateAsync(token);
        await dataCleaner.CleanAsync(token);
        await taskWorker.WorkAsync(token);
        await catalogPageRepository.LoadAsync();
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogWarning("Server is shutting down...");
        await networkClientRepository.DisposeAsync();
    }
}