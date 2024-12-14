using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Other;

namespace SadieEmulator.Tasks.Networking;

public class PingClientsTask(INetworkClientRepository clientRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(20);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        await clientRepository.BroadcastDataAsync(new PlayerPingWriter());
    }
}