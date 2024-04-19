using DotNetty.Transport.Channels;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Client;

public interface INetworkClient : IAsyncDisposable, INetworkObject
{
    IChannel Channel { get; set; }
    PlayerLogic? Player { get; set; }
    RoomUser? RoomUser { get; set; }
    Task ListenAsync();
    DateTime LastPing { get; set; }
    Task OnReceivedAsync(byte[] data);
}