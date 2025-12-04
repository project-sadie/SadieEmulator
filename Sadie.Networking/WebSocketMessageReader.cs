using System.Net.WebSockets;
using Sadie.API.Interfaces.Networking;

namespace Sadie.Networking;

public class WebSocketMessageReader : IWebSocketMessageReader
{
    public async Task<byte[]> ReadMessageAsync(WebSocket socket, CancellationToken token)
    {
        var buffer = new byte[4096];
            
        using var ms = new MemoryStream();
            
        while (true)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                return [];
            }
            
            ms.Write(buffer, 0, result.Count);
            
            if (result.EndOfMessage)
            {
                break;
            }
        }
            
        return ms.ToArray();
    }
}