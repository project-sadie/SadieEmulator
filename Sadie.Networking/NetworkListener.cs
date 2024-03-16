using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;

namespace Sadie.Networking;

public class NetworkListener : INetworkListener
{
    private readonly ILogger<NetworkListener> _logger;
    private readonly TcpListener _listener;
        
    private readonly INetworkClientRepository _clientRepository;
    private readonly INetworkClientFactory _clientFactory;

    public NetworkListener(
        ILogger<NetworkListener> logger, 
        TcpListener listener, 
        INetworkClientRepository clientRepository, 
        INetworkClientFactory clientFactory)
    {
        _logger = logger;
        _listener = listener;
            
        _clientRepository = clientRepository;
        _clientFactory = clientFactory;
    }

    public void Start()
    {
        _listener.Start();
    }

    private bool _listening = true;
        
    public async Task ListenAsync()
    {
        _logger.LogInformation("Networking is listening for connections");
            
        while (_listening)
        {
            var client = await _listener.AcceptTcpClientAsync();
            await AcceptClient(client);
        }
    }
        
    private async Task AcceptClient(TcpClient client)
    {
        var clientId = Guid.NewGuid();
        var networkClient = _clientFactory.CreateClient(clientId, client);
        
        _clientRepository.AddClient(clientId, networkClient);
                
        await networkClient.ListenAsync();
    }

    public void Dispose()
    {
        _listening = false;
            
        _listener.Server.Shutdown(SocketShutdown.Both);
        _listener.Server.Close();
    }
}