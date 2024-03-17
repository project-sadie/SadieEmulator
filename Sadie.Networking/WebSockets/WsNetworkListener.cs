using System.Net;
using System.Net.WebSockets;
using Sadie.Networking.Client;

namespace Sadie.Networking.WebSockets
{
    public class WsNetworkListener(
        INetworkClientRepository clientRepository,
        INetworkClientFactory clientFactory,
        HttpListener httpListener)
        : INetworkListener
    {
        public void Dispose()
        {
            if (httpListener.IsListening)
            {
                httpListener.Stop();
            }
            
            httpListener.Close();
        }
        
        public void Start()
        {
            ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
            httpListener.Start();
        }

        public async Task ListenAsync()
        {
            while (true)
            {
                var context = await httpListener.GetContextAsync();

                if (!context.Request.IsWebSocketRequest)
                {
                    continue;
                } 

                var guid = Guid.NewGuid();

                WebSocketContext wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                var ws = wsContext.WebSocket;

                var client = clientFactory.CreateClient(guid, ws);
                clientRepository.AddClient(guid, client);
                await client.ListenAsync();
            }
        }
    }
}
