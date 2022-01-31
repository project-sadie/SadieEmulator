using Sadie.Game.Players;

namespace Sadie.Networking.Client
{
    public interface INetworkClient : IDisposable
    {
        IPlayer? Player { get; set; }
        Task ListenAsync();
        Task WriteToStreamAsync(byte[] data);
    }
}