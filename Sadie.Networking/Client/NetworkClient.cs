using DotNetty.Transport.Channels;
using Microsoft.Extensions.Options;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Networking;
using Sadie.Networking.Codecs.Encryption;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization;
using Sadie.Options.Options;

namespace Sadie.Networking.Client;

public class NetworkClient(
    IChannel channel,
    IOptions<NetworkPacketOptions> options)
    : INetworkClient
{
    public IChannel Channel { get; set; } = channel;

    public IPlayerLogic? Player { get; set; }
    public IRoomUser? RoomUser { get; set; }
    public bool EncryptionEnabled { get; private set; }

    public void EnableEncryption(byte[] sharedKey)
    {
        Channel.Pipeline.AddFirst(new EncryptionDecoder(sharedKey));
        Channel.Pipeline.AddFirst(new EncryptionEncoder(sharedKey));

        EncryptionEnabled = true;
    }

    public DateTime LastPing { get; set; } = DateTime.Now;

    public async Task WriteToStreamAsync(AbstractPacketWriter writer)
    {
        if (!channel.IsWritable)
        {
            return;
        }

        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);

        try
        {
            await channel.WriteAndFlushAsync(serializedObject);
        }
        catch (ClosedChannelException)
        {
            
        }
        catch (ObjectDisposedException)
        {
            
        }
    }

    public async Task WriteToStreamAsync(INetworkPacketWriter writer)
    {
        if (!channel.IsWritable)
        {
            return;
        }

        try
        {
            await channel.WriteAndFlushAsync(writer);
        }
        catch (ClosedChannelException)
        {
            
        }
        catch (ObjectDisposedException)
        {
            
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
        
        await channel.CloseAsync();
    }
}