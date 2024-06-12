using Sadie.Networking.Serialization;

namespace Sadie.Shared.Unsorted.Networking;

public interface INetworkObject
{
    Task WriteToStreamAsync(AbstractPacketWriter writer);
    Task WriteToStreamAsync(NetworkPacketWriter writer);
}