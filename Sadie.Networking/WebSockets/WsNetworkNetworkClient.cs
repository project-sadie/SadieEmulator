using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.WebSockets;

public class WsNetworkNetworkClient : WsNetworkClientProcessComponent, INetworkClient
{
    public Guid Guid { get; private set; }

    private readonly ILogger<NetworkClient> _logger;
    private readonly WebSocket _webSocket;
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;

    public WsNetworkNetworkClient(
        ILogger<NetworkClient> logger, 
        Guid guid, 
        WebSocket webSocket, 
        INetworkPacketHandler packetHandler, 
        NetworkingConstants constants,
        IPlayerRepository playerRepository, 
        IRoomRepository roomRepository) : base(webSocket, packetHandler, constants)
    {
        Guid = guid;

        _logger = logger;
        _webSocket = webSocket;
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        
        SetClient(this);
    }

    public IPlayer? Player { get; set; }
    public RoomUser? RoomUser { get; set; }
    
    public Task ListenAsync()
    {
        var listeningThread = new Thread(StartListening)
        {
            Name = "Client Listening Thread",
            Priority = ThreadPriority.AboveNormal
        };
        
        listeningThread.Start();
        return Task.CompletedTask;
    }

    public DateTime LastPing { get; set; }

    public async Task WriteToStreamAsync(byte[] data)
    {
        await _webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    private bool _disposed;
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (RoomUser != null)
        {
            var (foundRoom, lastRoom) = _roomRepository.TryGetRoomById(RoomUser.AvatarData.CurrentRoomId);
            
            if (foundRoom && lastRoom != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(RoomUser.Id);
                RoomUser = null;
            }
        }

        if (!await _playerRepository.TryRemovePlayerAsync(Player.Data.Id))
        {
            _logger.LogError("Failed to dispose of player");
        }
        
        _webSocket.Dispose();
    }
}