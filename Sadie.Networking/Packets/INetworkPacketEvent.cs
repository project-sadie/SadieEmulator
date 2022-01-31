using Sadie.Networking.Client;

namespace Sadie.Networking.Packets;

public interface INetworkPacketEvent
{
    Task HandleAsync(INetworkClient client, INetworkPacketReader reader);
}