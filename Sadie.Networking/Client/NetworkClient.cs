using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Sadie.API;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Networking.Packets.Serialization;

namespace Sadie.Networking.Client;

public class NetworkClient(
    ILogger<NetworkClient> logger,
    IPAddress ipAddress,
    Guid guid,
    WebSocket webSocket)
    : INetworkClient
{
    public IPAddress IpAddress { get; set; } = ipAddress;
    public Guid Guid { get; set; } = guid;
    public WebSocket WebSocket { get; set; } = webSocket;

    public IPlayerLogic? Player { get; set; }
    public IRoomUser? RoomUser { get; set; }
    public bool EncryptionEnabled { get; private set; }

    public void EnableEncryption(byte[] sharedKey)
    {
        EncryptionEnabled = true;
    }

    public DateTime LastPing { get; set; } = DateTime.Now;
    public DateTime LastPong { get; set; } = DateTime.Now;

    public async Task WriteToStreamAsync(AbstractPacketWriter writer)
    {
        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        await WriteToStreamAsync(serializedObject);
    }

    public List<INetworkPacketWriter> Outbox { get; set; } = [];

    public async Task WriteToStreamAsync(INetworkPacketWriter writer)
    {
        try
        {
            _ = WebSocket.SendAsync(writer.GetAllBytes(), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            if (WebSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                await WebSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing",
                    CancellationToken.None
                );
            }
        }
        catch (WebSocketException) {}
    }
}