using System.Buffers.Binary;
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
        _buffer = new byte[8000];
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
                OnReceived(bytesReceived);
            }
        }
        catch (NullReferenceException)
        {
            Dispose();
        }
    }
    
    private void OnReceived(int bytesReceived)
    {
        try
        {
            var packet = DecodePacketFromBytes(bytesReceived, _buffer);

            if (_networkClient == null || packet == null)
            {
                return;
            }

            if (packet.PacketId == -1)
            {
                OnReceivedPolicyRequest();
                return;
            }
            
            _packetHandler.HandleAsync(_networkClient, packet).Wait();
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