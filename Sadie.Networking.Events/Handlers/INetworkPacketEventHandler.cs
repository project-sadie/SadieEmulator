using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers;

public interface INetworkPacketEventHandler
{
    Task HandleAsync(INetworkClient client);
}