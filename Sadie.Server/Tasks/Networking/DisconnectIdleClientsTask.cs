using Sadie.API.Networking.Client;

namespace SadieEmulator.Tasks.Networking;

public class DisconnectIdleClientsTask(INetworkClientRepository clientRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        await clientRepository.DisconnectIdleClientsAsync();
    }
}