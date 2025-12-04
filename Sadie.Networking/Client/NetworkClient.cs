using DotNetty.Transport.Channels;
using Sadie.API;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Networking.Codecs.Encryption;
using Sadie.Networking.Packets.Serialization;

namespace Sadie.Networking.Client;

public class NetworkClient(
    IChannel channel)
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
    public DateTime LastPong { get; set; } = DateTime.Now;

    public async Task WriteToStreamAsync(AbstractPacketWriter writer)
    {
        if (!Channel.IsWritable)
        {
            return;
        }

        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        _ = Channel.WriteAsync(serializedObject);
    }

    public List<INetworkPacketWriter> Outbox { get; set; } = [];

    public async Task WriteToStreamAsync(INetworkPacketWriter writer)
    {
        if (!Channel.IsWritable)
        {
            return;
        }

        try
        {
            _ = Channel.WriteAsync(writer);
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
        
        await Channel.CloseAsync();
    }
}