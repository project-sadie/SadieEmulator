using Sadie.API.Networking.Packets;

namespace Sadie.Networking.Packets;

public class NetworkPacket(short packetId, byte[] packetData) : NetworkPacketReader(packetData), INetworkPacket
{
    public short PacketId { get; } = packetId;
}
