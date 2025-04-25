using DotNetty.Transport.Channels;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;

namespace Sadie.Networking.Client;

public interface INetworkClient : IAsyncDisposable, INetworkObject
{
    IChannel Channel { get; set; }
    IPlayerLogic? Player { get; set; }
    IRoomUser? RoomUser { get; set; }
    bool EncryptionEnabled { get; }
    void EnableEncryption(byte[] sharedKey);
    DateTime LastPing { get; set; }
}