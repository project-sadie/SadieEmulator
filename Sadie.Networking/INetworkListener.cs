namespace Sadie.Networking;

public interface INetworkListener : IAsyncDisposable
{
    Task ListenAsync(int port);
}