using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClient : NetworkClientProcessComponent, INetworkClient
{
    private readonly Guid _guid;
    private readonly ILogger<NetworkClient> _logger;
    private readonly TcpClient _tcpClient;
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly INetworkClientRepository _clientRepository;

    public NetworkClient(
        Guid guid, 
        ILogger<NetworkClient> logger, 
        TcpClient tcpClient, 
        INetworkPacketHandler packetHandler, 
        ILogger<NetworkClientProcessComponent> baseLogger, 
        NetworkingConstants constants,
        IPlayerRepository playerRepository, 
        IRoomRepository roomRepository,
        INetworkClientRepository clientRepository) : base(baseLogger, tcpClient, packetHandler, constants)
    {
        _guid = guid;
        _logger = logger;
        _tcpClient = tcpClient;
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _clientRepository = clientRepository;

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

        if (Player != null && await _playerRepository.TryRemovePlayerAsync(Player.Data.Id))
        {
            Player = null;
        }

        if (!await _clientRepository.TryRemoveAsync(_guid))
        {
            _logger.LogError("Failed to dispose of network client");
        }

        if (_tcpClient.Connected)
        {
            _tcpClient.GetStream().Close();
            _tcpClient.Close();
        }
    }
}