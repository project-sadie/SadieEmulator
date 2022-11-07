using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClientProcessComponent : NetworkPacketDecoder
{
    private readonly ILogger<NetworkClientProcessComponent> _logger;
    private readonly TcpClient _client;
    
    private readonly INetworkPacketHandler _packetHandler;
    private readonly INetworkClientRepository _clientRepository;
    private readonly byte[] _buffer;

    protected NetworkClientProcessComponent(ILogger<NetworkClientProcessComponent> logger, TcpClient client, INetworkPacketHandler packetHandler, NetworkingConstants constants, INetworkClientRepository clientRepository) : base(constants)
    {
        _logger = logger;
        _client = client;
        
        _packetHandler = packetHandler;
        _clientRepository = clientRepository;
        _buffer = new byte[constants.BufferByteSize];
    }

    protected async void StartListening()
    {
        try
        {
            while (_client.Connected)
            {
                var bytes = await _client.Client.ReceiveAsync(_buffer, SocketFlags.None);

                if (bytes > 0)
                {
                    await OnReceivedAsync(bytes);
                }
            }
        }
        catch (SocketException)
        {
            if (_networkClient != null && !await _clientRepository.TryRemoveAsync(_networkClient.Guid))
            {
                _logger.LogError("Failed to dispose of network client");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }
    
    private INetworkClient? _networkClient;

    protected void SetClient(INetworkClient client) => _networkClient = client;

    private async Task OnReceivedAsync(int bytesReceived)
    {
        var data = new byte[bytesReceived];
        Buffer.BlockCopy(_buffer, 0, data, 0, bytesReceived);

        if (data[0] == 60)
        {
            await OnReceivedPolicyRequest();
        }
        else if (_networkClient != null)
        {
            foreach (var packet in DecodePacketsFromBytes(data))
            {
                _packetHandler.HandleAsync(_networkClient, packet);
            }
        }
    }

    private async Task OnReceivedPolicyRequest()
    {
        await _networkClient?.WriteToStreamAsync(Encoding.Default.GetBytes("<?xml version=\"1.0\"?>\r\n" +
           "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
           "<cross-domain-policy>\r\n" +
           "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
           "</cross-domain-policy>\x0"))!;
    }
}