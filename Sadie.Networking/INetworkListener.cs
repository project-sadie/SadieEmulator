namespace Sadie.Networking;

public interface INetworkListener : IDisposable
{
    void Initialize();
    Task ListenAsync();
}