using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
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
    private readonly INetworkPacketHandler _packetHandler;

    public NetworkClient(
        ILogger<NetworkClient> logger,
        IChannel channel,
        INetworkPacketHandler packetHandler,
        IOptions<NetworkPacketOptions> options) : base(options)
    {
        Channel = channel;
        
        _logger = logger;
        _channel = channel;
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

    public DateTime LastPing { get; set; } = DateTime.Now;

    public async Task WriteToStreamAsync(AbstractPacketWriter writer)
    {
        if (!_channel.IsWritable)
        {
            return;
        }

        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        await _channel.WriteAndFlushAsync(serializedObject);
    }

    public async Task WriteToStreamAsync(NetworkPacketWriter writer)
    {
        if (!_channel.IsWritable)
        {
            return;
        }

        try
        {
            await _channel.WriteAndFlushAsync(writer);
        }
        catch (ClosedChannelException)
        {
            
        }
        catch (ObjectDisposedException)
        {
            
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
        
        await _channel.CloseAsync();
    }
}