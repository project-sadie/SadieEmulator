namespace Sadie.Networking.Packets;

public interface INetworkPacket
{
    short PacketId { get; }
    byte[] Data { get; }
}