using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Packets;
using Sadie.Shared;

namespace Sadie.Networking.Client;

public class NetworkClientProcessComponent : NetworkPacketDecoder, IDisposable
{
    private readonly ILogger<NetworkClientProcessComponent> _logger;
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    
    private readonly INetworkPacketHandler _packetHandler;
    private readonly byte[] _buffer;

    protected NetworkClientProcessComponent(ILogger<NetworkClientProcessComponent> logger, TcpClient client, INetworkPacketHandler packetHandler)
    {
        _logger = logger;
        _client = client;
        _stream = client.GetStream();
        
        _packetHandler = packetHandler;
        _buffer = new byte[SadieConstants.HabboPacketBufferSize];
    }

    protected async Task StartListening(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var bytes = await _client.Client.ReceiveAsync(_buffer, SocketFlags.None);

                if (bytes > 0)
                {
                    await OnReceivedAsync(bytes);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            Dispose();
        }
    }
    
    private INetworkClient? _networkClient;

    protected void SetClient(INetworkClient client) => _networkClient = client;

    private Task OnReceivedAsync(int bytesReceived)
    {
        try
        {
            var data = new byte[bytesReceived];
            Buffer.BlockCopy(_buffer, 0, data, 0, bytesReceived);
            
            if (data[0] == 60)
            {
                OnReceivedPolicyRequest();
            }
            else if (_networkClient != null)
            {
                foreach (var packet in DecodePacketsFromBytes(data))
                {
                    _packetHandler.HandleAsync(_networkClient, packet).Wait();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            Dispose();
        }

        return Task.CompletedTask;
    }

    private void OnReceivedPolicyRequest()
    {
        WriteToStreamAsync(Encoding.Default.GetBytes(SadieConstants.CrossDomainPolicy)).Wait();
    }

    public async Task WriteToStreamAsync(byte[] data)
    {
        await _stream.WriteAsync(data);
    }

    public void Dispose()
    {
        _client.Close();
        _networkClient?.Dispose();
    }
}