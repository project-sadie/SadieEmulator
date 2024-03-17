using System.Net.WebSockets;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.WebSockets;

public class WsNetworkClientProcessComponent : NetworkPacketDecoder
{
    private readonly WebSocket _webSocket;
    private readonly INetworkPacketHandler _packetHandler;
    private readonly INetworkClientRepository _clientRepository;
    private readonly byte[] _buffer;

    protected WsNetworkClientProcessComponent(WebSocket webSocket, INetworkPacketHandler packetHandler, NetworkingConstants constants, 
        INetworkClientRepository clientRepository) : base(constants)
    {
        _webSocket = webSocket;
        _packetHandler = packetHandler;
        _clientRepository = clientRepository;
        _buffer = new byte[constants.BufferByteSize];
    }

    private async Task<ArraySegment<byte>> MessageReadAsync()
    {
        using (var ms = new MemoryStream())
        {
            var seg = new ArraySegment<byte>(_buffer);

            while (true)
            {
                var result = await _webSocket.ReceiveAsync(seg, CancellationToken.None).ConfigureAwait(false);
                
                if (result.CloseStatus != null || _webSocket.State != WebSocketState.Open)
                {
                    throw new WebSocketException();
                }

                if (result.Count > 0)
                {
                    ms.Write(_buffer, 0, result.Count);
                }

                if (result.EndOfMessage)
                {
                    return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
                }
            }
        } 
    }
    
    protected async void StartListening()
    {
        while (true)
        {
            try
            {
                var data = await MessageReadAsync().ConfigureAwait(false);
                await OnReceivedAsync(data.ToArray());
            }
            catch (WebSocketException)
            {
                await _clientRepository.TryRemoveAsync(_networkClient.Guid);
            }
        }
    }
    
    private INetworkClient? _networkClient;

    protected void SetClient(INetworkClient client) => _networkClient = client;
    
    public Task OnReceivedAsync(byte[] data)
    {
        if (_networkClient == null)
        {
            return Task.CompletedTask;
        }
        
        foreach (var packet in DecodePacketsFromBytes(data))
        {
            _packetHandler.HandleAsync(_networkClient, packet);
        }
        
        return Task.CompletedTask;
    }
}