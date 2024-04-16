using Fleck;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.Networking.Client;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Sadie.Options.Options;

namespace Sadie.Networking
{
    public class NetworkListener(
        ILogger<NetworkListener> logger,
        INetworkClientRepository clientRepository,
        INetworkClientFactory clientFactory,
        IOptions<NetworkOptions> options)
        : INetworkListener
    {
        private readonly NetworkOptions networkSettings = options.Value;
        private WebSocketServer? server;

        public void Initialize()
        {
            // Shut fleck up 
            FleckLog.LogAction = (x, y, z) => { };

            var location = networkSettings.UseWss ? 
                $"wss://{networkSettings.Host}:{networkSettings.Port}" : 
                $"ws://{networkSettings.Host}:{networkSettings.Port}";
            
            server = new WebSocketServer(location, networkSettings.UseWss);
            var certificateLocation = networkSettings.CertificateFile;

            if (networkSettings.UseWss && !string.IsNullOrEmpty(certificateLocation))
            {
                server.Certificate = new X509Certificate2(certificateLocation, "");
            }

            server.EnabledSslProtocols = SslProtocols.Tls12;
        }

        public Task ListenAsync()
        {
            server?.Start(socket =>
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
            server?.Dispose();
        }
    }
}
