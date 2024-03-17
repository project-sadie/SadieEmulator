using Sadie.Networking.Client;

namespace SadieEmulator.Tasks.Networking;

public class DisconnectIdleClientsTask(INetworkClientRepository clientRepository) : IServerTask
{
    public string Name => "DisconnectIdleClientsTask";
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(20);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        await clientRepository
            .DisconnectIdleClientsAsync();
    }
}