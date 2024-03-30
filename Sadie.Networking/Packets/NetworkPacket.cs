namespace Sadie.Networking.Packets;

public class NetworkPacket(int packetId, byte[] packetData) : NetworkPacketReader(packetData), INetworkPacket
{
    public int PacketId { get; } = packetId;
}
