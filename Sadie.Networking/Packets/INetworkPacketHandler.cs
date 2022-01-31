using Sadie.Networking.Client;

namespace Sadie.Networking.Packets;

public interface INetworkPacketHandler
{ 
    Task HandleAsync(INetworkClient client, INetworkPacket packet);
}