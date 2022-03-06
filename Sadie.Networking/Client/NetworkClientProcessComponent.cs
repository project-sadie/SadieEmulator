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
                if (_networkClient == null || _networkClient.LastPing != default && DateTime.Now.Subtract(_networkClient.LastPing).TotalSeconds >= 60)
                {
                    _networkClient?.Dispose();
                    break;
                }
                
                cancellationToken.ThrowIfCancellationRequested();
                
                var bytes = await _client.Client.ReceiveAsync(_buffer, SocketFlags.None);

                if (bytes > 0)
                {
                    await OnReceivedAsync(bytes);
                }
                
                Thread.Sleep(50);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            _networkClient?.Dispose();
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
                    await _packetHandler.HandleAsync(_networkClient, packet);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            _networkClient?.Dispose();
        }
    }

    private async Task OnReceivedPolicyRequest()
    {
        await WriteToStreamAsync(Encoding.Default.GetBytes(SadieConstants.CrossDomainPolicy));
    }

    public async Task WriteToStreamAsync(byte[] data)
    {
        try
        {
            await _stream.WriteAsync(data);
        }
        catch (IOException)
        {
            _logger.LogError("Failed to write data to the stream.");
        }
    }

    public void Dispose()
    {
        _client.GetStream().Close();
        _client.Close();
    }
}