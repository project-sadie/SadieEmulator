using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Server.Tasks;

namespace Sadie.Server.Tasks.Networking;

public class DisconnectIdleClientsTask(INetworkClientRepository clientRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(20);
    public long LastExecutedTicks { get; set; }

    public async Task ExecuteAsync()
    {
        await clientRepository.DisconnectIdleClientsAsync();
    }
}