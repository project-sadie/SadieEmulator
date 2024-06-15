using Sadie.Networking.Client;

namespace Sadie.Networking.Events.Handlers;

public interface INetworkPacketEventHandler
{
    Task HandleAsync(INetworkClient client);
}