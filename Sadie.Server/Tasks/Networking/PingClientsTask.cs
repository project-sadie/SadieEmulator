using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Other;

namespace SadieEmulator.Tasks.Networking;

public class PingClientsTask(INetworkClientRepository clientRepository) : AbstractTask
{
    public override TimeSpan PeriodicInterval => TimeSpan.FromSeconds(20);

    public override async Task ExecuteAsync()
    {
        await clientRepository.BroadcastDataAsync(new PlayerPingWriter());
    }
}