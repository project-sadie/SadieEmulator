namespace Sadie.Networking;

public interface INetworkListener : IDisposable
{
    void Start(int backlog = 100);
    Task ListenAsync();
}