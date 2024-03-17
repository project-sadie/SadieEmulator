using System.Net;
using System.Net.WebSockets;
using Sadie.Networking.Client;

namespace Sadie.Networking.WebSockets
{
    public class WsNetworkListener : INetworkListener
    {
        private readonly INetworkClientRepository _clientRepository;
        private readonly INetworkClientFactory _clientFactory;
        private readonly HttpListener _httpListener;

        public WsNetworkListener(
            INetworkClientRepository clientRepository, 
            INetworkClientFactory clientFactory, 
            HttpListener httpListener)
        {
            _clientRepository = clientRepository;
            _clientFactory = clientFactory;
            _httpListener = httpListener;
        }

        public void Dispose()
        {
            if (_httpListener.IsListening)
            {
                _httpListener.Stop();
            }
            
            _httpListener.Close();
        }
        
        public void Start()
        {
            ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
            _httpListener.Start();
        }

        public async Task ListenAsync()
        {
            while (true)
            {
                var context = await _httpListener.GetContextAsync();

                if (!context.Request.IsWebSocketRequest)
                {
                    continue;
                } 

                var guid = Guid.NewGuid();

                WebSocketContext wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                var ws = wsContext.WebSocket;

                var client = _clientFactory.CreateClient(guid, ws);
                _clientRepository.AddClient(guid, client);
                await client.ListenAsync();
            }
        }
    }
}
