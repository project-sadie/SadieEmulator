using Sadie.Networking.Serialization;

namespace Sadie.API;

public interface INetworkObject
{
    Task WriteToStreamAsync(AbstractPacketWriter writer);
    Task WriteToStreamAsync(NetworkPacketWriter writer);
}