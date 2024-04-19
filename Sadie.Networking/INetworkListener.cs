namespace Sadie.Networking;

public interface INetworkListener : IAsyncDisposable
{
    void Bootstrap();
    Task ListenAsync();
}