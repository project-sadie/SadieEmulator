namespace Sadie.Networking.Client;

public interface INetworkClientRepository
{
    void AddClient(Guid guid, INetworkClient client);
    Task<bool> TryRemoveAsync(Guid clientId);
    Task DisconnectIdleClientsAsync();
    bool TryGetClientByGuid(Guid guid, out INetworkClient? client);
}