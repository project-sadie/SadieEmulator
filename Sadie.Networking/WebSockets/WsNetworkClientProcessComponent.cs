using System.Net.WebSockets;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.WebSockets;

public class WsNetworkClientProcessComponent : NetworkPacketDecoder
{
    private readonly WebSocket _webSocket;
    private readonly INetworkPacketHandler _packetHandler;
    private readonly byte[] _buffer;

    protected WsNetworkClientProcessComponent(WebSocket webSocket, INetworkPacketHandler packetHandler, NetworkingConstants constants) : base(constants)
    {
        _webSocket = webSocket;
        _packetHandler = packetHandler;
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
                if (result.CloseStatus != null)
                {
                    Console.WriteLine("close received");
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    throw new WebSocketException("Websocket closed.");
                }

                if (_webSocket.State != WebSocketState.Open)
                {
                    Console.WriteLine("websocket no longer open");
                    throw new WebSocketException("Websocket closed.");
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
            var data = await MessageReadAsync().ConfigureAwait(false);
            await OnReceivedAsync(data.ToArray());
        }
    }
    
    private INetworkClient? _networkClient;

    protected void SetClient(INetworkClient client) => _networkClient = client;
    
    public Task OnReceivedAsync(byte[] data)
    {
        foreach (var packet in DecodePacketsFromBytes(data))
        {
            _packetHandler.HandleAsync(_networkClient, packet);
        }
        
        return Task.CompletedTask;
    }
}