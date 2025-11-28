using Sadie.API.Interfaces.Networking.Client;

namespace Sadie.Networking.Packets;

public interface INetworkPacketHandler
{ 
    Task HandleAsync(INetworkClient client, INetworkPacket packet);
}