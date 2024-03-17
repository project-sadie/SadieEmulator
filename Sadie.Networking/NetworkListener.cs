using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;

namespace Sadie.Networking;

public class NetworkListener(
    ILogger<NetworkListener> logger,
    TcpListener listener,
    INetworkClientRepository clientRepository,
    INetworkClientFactory clientFactory)
    : INetworkListener
{
    public void Start()
    {
        listener.Start();
    }

    private bool _listening = true;
        
    public async Task ListenAsync()
    {
        logger.LogInformation("Networking is listening for connections");
            
        while (_listening)
        {
            var client = await listener.AcceptTcpClientAsync();
            await AcceptClient(client);
        }
    }
        
    private async Task AcceptClient(TcpClient client)
    {
        var clientId = Guid.NewGuid();
        var networkClient = clientFactory.CreateClient(clientId, client);
        
        clientRepository.AddClient(clientId, networkClient);
                
        await networkClient.ListenAsync();
    }

    public void Dispose()
    {
        _listening = false;
            
        listener.Server.Shutdown(SocketShutdown.Both);
        listener.Server.Close();
    }
}