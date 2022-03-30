using Sadie.Networking.Client;

namespace SadieEmulator.Tasks.Networking;

public class DisconnectIdleClientsTask : IServerTask
{
    private readonly INetworkClientRepository _clientRepository;
    public string Name => "DisconnectIdleClientsTask";
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(20);
    public DateTime LastExecuted { get; set; }
    
    public DisconnectIdleClientsTask(INetworkClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task ExecuteAsync()
    {
        await _clientRepository
            .DisconnectIdleClientsAsync();
    }
}