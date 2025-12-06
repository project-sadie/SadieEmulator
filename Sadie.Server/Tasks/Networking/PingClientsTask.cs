using Sadie.API.Interfaces.Networking.Client;
using Sadie.Networking.Packets.Writers.Players.Other;

namespace SadieEmulator.Tasks.Networking;

public class PingClientsTask(INetworkClientRepository networkClientRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    public long LastExecutedTicks { get; set; }
    
    public async Task ExecuteAsync()
    {
        foreach (var client in networkClientRepository.Clients)
        {
            await client.WriteToStreamAsync(new PlayerPingWriter());
            client.LastPing = DateTime.Now;
        }
    }
}