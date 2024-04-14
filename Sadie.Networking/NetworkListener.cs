using System.Security.Authentication;
using Fleck;
using Microsoft.Extensions.Logging;
using Sadie.Networking.Client;

namespace Sadie.Networking
{
    public class NetworkListener(
        ILogger<NetworkListener> logger,
        INetworkClientRepository clientRepository,
        INetworkClientFactory clientFactory,
        WebSocketServer server)
        : INetworkListener
    {
        public void Start()
        {
            server.EnabledSslProtocols = SslProtocols.Tls12;
        }

        public Task ListenAsync()
        {
            server.Start(socket =>
            {
                socket.OnOpen = () => OnOpen(socket);
                socket.OnClose = () => OnClose(socket);
                socket.OnBinary = message => OnBinary(socket, message);
            });
            
            logger.LogInformation("Networking is listening for connections");
            return Task.CompletedTask;
        }

        private void OnOpen(IWebSocketConnection connection)
        {
            try
            {
                var client = clientFactory.CreateClient(connection.ConnectionInfo.Id, connection);
                clientRepository.AddClient(connection.ConnectionInfo.Id, client);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }

        private async void OnClose(IWebSocketConnection connection)
        {
            if (!await clientRepository.TryRemoveAsync(connection.ConnectionInfo.Id))
            {
                logger.LogError("Failed to remove client from connection information.");
            }
        }

        private void OnBinary(IWebSocketConnection connection, byte[] message)
        {
            if (!clientRepository.TryGetClientByGuid(connection.ConnectionInfo.Id, out var client))
            {
                logger.LogError($"Failed to resolve client from connection information.");
            }
            else
            {
                client!.OnReceivedAsync(message);
            }
        }
        
        public void Dispose()
        {
            server.Dispose();
        }
    }
}
