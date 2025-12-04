using Sadie.API.Interfaces.Networking.Packets;

namespace Sadie.Networking.Packets;

public class NetworkPacket(short header, byte[] data) : INetworkPacket
{
    public short PacketId { get; } = header;
    public byte[] Data { get; } = data;
}
