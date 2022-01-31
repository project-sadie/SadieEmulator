namespace Sadie.Networking.Packets;

public class NetworkPacket : NetworkPacketReader, INetworkPacket
{
    public int PacketId { get; }

    public NetworkPacket(int packetId, byte[] packetData) : base(packetData)
    {
        PacketId = packetId;
    }
}
