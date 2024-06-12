using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Codecs.Encryption;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization;
using Sadie.Options.Options;

namespace Sadie.Networking.Client;

public class NetworkClient : NetworkPacketDecoder, INetworkClient
{
    public IChannel Channel { get; set; }

    private readonly ILogger<NetworkClient> _logger;
    private readonly IChannel _channel;
    private readonly PlayerRepository _playerRepository;
    private readonly RoomRepository _roomRepository;
    private readonly INetworkPacketHandler _packetHandler;

    public NetworkClient(
        ILogger<NetworkClient> logger,
        IChannel channel,
        PlayerRepository playerRepository,
        RoomRepository roomRepository,
        INetworkPacketHandler packetHandler,
        IOptions<NetworkPacketOptions> options) : base(options)
    {
        Channel = channel;

        _logger = logger;
        _channel = channel;
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _packetHandler = packetHandler;
    }

    public PlayerLogic? Player { get; set; }
    public IRoomUser? RoomUser { get; set; }
    public bool EncryptionEnabled { get; private set; }

    public Task ListenAsync()
    {
        return Task.CompletedTask;
    }

    public void EnableEncryption(byte[] sharedKey)
    {
        Channel.Pipeline.AddFirst(new EncryptionDecoder(sharedKey));
        Channel.Pipeline.AddFirst(new EncryptionEncoder(sharedKey));

        EncryptionEnabled = true;
    }

    public DateTime LastPing { get; set; }

    public async Task WriteToStreamAsync(AbstractPacketWriter writer)
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
            await _channel.WriteAndFlushAsync(serializedObject);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error whilst writing to stream: {e}");
        }
    }

    public async Task WriteToStreamAsync(NetworkPacketWriter writer)
    {
        await _channel.WriteAndFlushAsync(writer);
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
            var lastRoom = _roomRepository.TryGetRoomById(RoomUser.Player.CurrentRoomId);

            if (lastRoom != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(RoomUser.Id);
                RoomUser = null;
            }
        }

        if (Player != null && !await _playerRepository.TryRemovePlayerAsync(Player.Id))
        {
            _logger.LogError("Failed to dispose of player");
        }

        await _channel.CloseAsync();
    }
}