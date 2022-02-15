using Sadie.Game.Players;
using Sadie.Shared;
using Sadie.Shared.Networking;

namespace Sadie.Networking.Client;

public interface INetworkClient : IDisposable, INetworkObject
{
    IPlayer Player { get; set; }
    Task ListenAsync();
}