namespace Sadie.Networking;

public interface INetworkListener : IDisposable
{
    void Start();
    Task ListenAsync();
}