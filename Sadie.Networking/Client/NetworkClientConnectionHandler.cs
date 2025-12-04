using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Networking;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Packets;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClientConnectionHandler(
    ILogger<NetworkClientConnectionHandler> logger,
    INetworkClientRepository clientRepository,
    IWebSocketMessageReader webSocketMessageReader,
    INetworkPacketDecoder packetDecoder,
    PacketDispatcher packetDispatcher)
    : INetworkClientConnectionHandler
{
    public async Task HandleClientAsync(INetworkClient client, CancellationToken ct)
    {
        clientRepository.AddClient(client.Guid, client);

        logger.LogInformation($"Client {client.Guid} connected from {client.IpAddress}.");

        try
        {
            var socket = client.WebSocket;

            while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                var rawMessage = await webSocketMessageReader.ReadMessageAsync(socket, ct);

                if (rawMessage.Length == 0)
                {
                    continue;
                }

                var packet = packetDecoder.Decode(client.Guid, rawMessage);

                if (packet == null)
                {
                    continue;
                }
                
                packetDispatcher.Enqueue(client, packet);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (WebSocketException wsException)
        {
            logger.LogWarning(wsException, "Client {Guid} disconnected unexpectedly.", client.Guid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error handling client {Guid}.", client.Guid);
        }
        finally
        {
            await clientRepository.TryRemoveAsync(client.Guid);
            await client.DisposeAsync();

            logger.LogInformation("Client {Guid} disconnected.", client.Guid);
        }
    }
}