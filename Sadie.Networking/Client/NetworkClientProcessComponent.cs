using System.Net.Sockets;
using System.Text;
using Sadie.Networking.Packets;
using Sadie.Shared;

namespace Sadie.Networking.Client;

public class NetworkClientProcessComponent : NetworkPacketDecoder, IDisposable
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    
    private readonly INetworkPacketHandler _packetHandler;
    private readonly byte[] _buffer; 

    protected NetworkClientProcessComponent(TcpClient client, INetworkPacketHandler packetHandler)
    {
        _client = client;
        _stream = client.GetStream();
        
        _packetHandler = packetHandler;
        _buffer = new byte[SadieConstants.HabboPacketBufferSize];
    }

    protected void StartListening()
    {
        _client.Client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceived, _client);
    }
    
    private INetworkClient? _networkClient;

    protected void SetClient(INetworkClient client) => _networkClient = client;
    
    private void OnReceived(IAsyncResult iar)
    {
        try
        {
            var bytesReceived = _client.Client.EndReceive(iar);

            if (bytesReceived > 0)
            {
                OnReceivedByteCount(bytesReceived);
            }
        }
        catch (NullReferenceException)
        {
            Dispose();
        }
    }
    
    private void OnReceivedByteCount(int bytesReceived)
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
            Dispose();
        }
        finally
        {
            _client.Client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceived, _client);
        }
    }

    private void OnReceivedPolicyRequest()
    {
        WriteToStreamAsync(Encoding.Default.GetBytes(SadieConstants.HabboPolicyXml)).Wait();
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