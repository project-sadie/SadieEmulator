using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted.Networking;

public interface INetworkObject
{
    Task WriteToStreamAsync(NetworkPacketWriter data);
}