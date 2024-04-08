using Fleck;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClient : NetworkPacketDecoder, INetworkClient
{
    public Guid Guid { get; private set; }

    private readonly ILogger<NetworkClient> _logger;
    private readonly IWebSocketConnection _webSocket;
    private readonly PlayerRepository _playerRepository;
    private readonly RoomRepository _roomRepository;
    private readonly INetworkPacketHandler _packetHandler;

    public NetworkClient(
        ILogger<NetworkClient> logger, 
        Guid guid, 
        IWebSocketConnection webSocket,
        PlayerRepository playerRepository, 
        RoomRepository roomRepository,
        INetworkPacketHandler packetHandler,
        NetworkingConstants constants) : base(constants)
    {
        Guid = guid;

        _logger = logger;
        _webSocket = webSocket;
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _packetHandler = packetHandler;
    }

    public PlayerLogic? Player { get; set; }
    public RoomUser? RoomUser { get; set; }
    
    public Task ListenAsync()
    {
        return Task.CompletedTask;
    }

    public DateTime LastPing { get; set; }

    public async Task WriteToStreamAsync(byte[] data)
    {
        if (_disposed)
        {
            return;
        }
        
        try
        {
            await _webSocket.Send(data);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error whilst writing to stream: {e.Message}");
        }
    }
    
    public async Task OnReceivedAsync(byte[] data)
    {
        foreach (var packet in DecodePacketsFromBytes(data))
        {
            await _packetHandler.HandleAsync(this, packet);
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

        if (!await _playerRepository.TryRemovePlayerAsync(Player.Id))
        {
            _logger.LogError("Failed to dispose of player");
        }
        
        _webSocket.Close();
    }
}