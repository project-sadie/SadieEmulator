using DotNetty.Transport.Channels;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Client;

public interface INetworkClient : IAsyncDisposable, INetworkObject
{
    IChannel Channel { get; set; }
    PlayerLogic? Player { get; set; }
    IRoomUser? RoomUser { get; set; }
    bool EncryptionEnabled { get; }
    void EnableEncryption(byte[] sharedKey);
    DateTime LastPing { get; set; }
}