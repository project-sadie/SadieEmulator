using System.Buffers.Binary;
using Sadie.Shared;

namespace Sadie.Networking.Packets;

public class NetworkPacketDecoder
{
    protected static NetworkPacket? DecodePacketFromBytes(int bytesReceived, byte[] buffer)
    {
        var packet = new byte[bytesReceived];
        Array.Copy(buffer, packet, bytesReceived);

        if (packet[0] == 60)
        {
            return new NetworkPacket(-1, Array.Empty<byte>());
        }

        if (packet.Length < SadieConstants.HabboPacketMinLength || packet.Length > SadieConstants.HabboPacketMaxLength)
        {
            return null;
        }

        using var reader = new BinaryReader(new MemoryStream(packet));
        var packetLength = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));

        var packetData = reader.ReadBytes(packetLength);

        using var br2 = new BinaryReader(new MemoryStream(packetData));
        var packetId = BinaryPrimitives.ReadInt16BigEndian(br2.ReadBytes(2));

        var content = new byte[packetData.Length - 2];
        Buffer.BlockCopy(packetData, 2, content, 0, packetData.Length - 2);

        return new NetworkPacket(packetId, content);
    }
}