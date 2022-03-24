using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Packets;
using Sadie.Shared;

namespace Sadie.Networking.Client;

public class NetworkClientProcessComponent : NetworkPacketDecoder
{
    private readonly ILogger<NetworkClientProcessComponent> _logger;
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    
    private readonly INetworkPacketHandler _packetHandler;
    private readonly byte[] _buffer;

    protected NetworkClientProcessComponent(ILogger<NetworkClientProcessComponent> logger, TcpClient client, INetworkPacketHandler packetHandler, NetworkingConstants constants) : base(constants)
    {
        _logger = logger;
        _client = client;
        _stream = client.GetStream();
        
        _packetHandler = packetHandler;
        _buffer = new byte[constants.BufferByteSize];
    }

    protected async void StartListening()
    {
        try
        {
            while (true)
            {
                var bytes = await _client.Client.ReceiveAsync(_buffer, SocketFlags.None);

                if (bytes > 0)
                {
                    await OnReceivedAsync(bytes);
                }

                Thread.Sleep(50);
            }
        }
        catch (SocketException e)
        {
            _networkClient.DisposeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            _networkClient.DisposeAsync();
        }
    }
    
    private INetworkClient? _networkClient;

    protected void SetClient(INetworkClient client) => _networkClient = client;

    private async Task OnReceivedAsync(int bytesReceived)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            
            if (_networkClient != null)
            {
                await _networkClient.DisposeAsync();
            }
        }
    }

    private async Task OnReceivedPolicyRequest()
    {
        await WriteToStreamAsync(Encoding.Default.GetBytes("<?xml version=\"1.0\"?>\r\n" +
                                                           "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                                                           "<cross-domain-policy>\r\n" +
                                                           "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                                                           "</cross-domain-policy>\x0"));
    }

    private bool hasErrored;
    
    public async Task WriteToStreamAsync(byte[] data)
    {
        try
        {
            await _stream.WriteAsync(data);
        }
        catch (Exception)
        {
            if (_networkClient != null)
            {
                await _networkClient.DisposeAsync();
            }
        }
    }
}