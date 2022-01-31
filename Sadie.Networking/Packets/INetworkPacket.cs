namespace Sadie.Networking.Packets;

public interface INetworkPacket : INetworkPacketReader
{
    int PacketId { get; }
}