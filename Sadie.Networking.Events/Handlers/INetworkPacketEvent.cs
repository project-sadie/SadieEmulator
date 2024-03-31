using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers;

public interface INetworkPacketEvent
{
    Task HandleAsync(INetworkClient client, INetworkPacketReader reader);
}