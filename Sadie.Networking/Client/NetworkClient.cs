using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClient : NetworkClientProcessComponent, INetworkClient
{
    public Guid Guid { get; }
    
    private readonly ILogger<NetworkClient> _logger;
    private readonly TcpClient _tcpClient;
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly Stream _stream;

    public NetworkClient(
        Guid guid, 
        ILogger<NetworkClient> logger, 
        TcpClient tcpClient, 
        INetworkPacketHandler packetHandler, 
        ILogger<NetworkClientProcessComponent> baseLogger, 
        NetworkingConstants constants,
        IPlayerRepository playerRepository, 
        IRoomRepository roomRepository,
        INetworkClientRepository clientRepository) : base(baseLogger, tcpClient, packetHandler, constants, clientRepository)
    {
        Guid = guid;
        
        _logger = logger;
        _tcpClient = tcpClient;
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _stream = tcpClient.GetStream();

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
        try
        {
            await _stream.WriteAsync(data);
        }
        catch (Exception)
        {
            await DisposeAsync();
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

        if (_tcpClient.Connected)
        {
            _tcpClient.GetStream().Close();
            _tcpClient.Close();
        }
    }
}