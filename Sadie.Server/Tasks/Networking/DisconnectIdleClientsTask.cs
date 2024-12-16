using Sadie.Networking.Client;

namespace SadieEmulator.Tasks.Networking;

public class DisconnectIdleClientsTask(INetworkClientRepository clientRepository) : AbstractTask
{
    public override TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    
    public override async Task ExecuteAsync()
    {
        await clientRepository.DisconnectIdleClientsAsync();
    }
}