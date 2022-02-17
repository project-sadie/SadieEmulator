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

    public NetworkListener(ILogger<NetworkListener> logger, TcpListener listener, INetworkClientRepository clientRepository, INetworkClientFactory clientFactory)
    {
        _logger = logger;
        _listener = listener;
            
        _clientRepository = clientRepository;
        _clientFactory = clientFactory;
    }

    public void Start(int backlog = 100)
    {
        _listener.Start(backlog);
    }

    private bool _listening = true;
        
    public async Task ListenAsync()
    {
        _logger.LogInformation("Networking is listening for connections.");
            
        while (_listening)
        {
            if (!_listener.Pending())
            {
                Thread.Sleep(100);
                continue;
            }
                
            var client = await _listener.AcceptTcpClientAsync();
            await AcceptClient(client);
        }
    }
        
    private async Task AcceptClient(TcpClient client)
    {
        var clientId = new Guid();
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