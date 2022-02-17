namespace Sadie.Networking.Client;

public interface INetworkClientRepository
{
    void AddClient(Guid guid, INetworkClient client);
    bool TryRemove(Guid clientId);
}