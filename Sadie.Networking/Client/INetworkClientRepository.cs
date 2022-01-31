namespace Sadie.Networking.Client;

public interface INetworkClientRepository
{
    void AddClient(INetworkClient client);
    bool TryRemove(int clientId);
}