using System.Buffers.Binary;

namespace Sadie.Networking.Packets;

public class NetworkPacketDecoder
{
    private readonly NetworkingConstants _constants;
    
    public NetworkPacketDecoder(NetworkingConstants constants)
    {
        _constants = constants;
    }

    protected List<NetworkPacket> DecodePacketsFromBytes(byte[] packet)
    {
        // TODO: Refactor this :(
        
        if (packet.Length < _constants.FrameLengthByteCount || packet.Length > _constants.BufferByteSize - _constants.FrameLengthByteCount)
        {
            return new List<NetworkPacket>();
        }

        using var reader = new BinaryReader(new MemoryStream(packet));
        var packetLength = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));

        var packetData = reader.ReadBytes(packetLength);

        using var br2 = new BinaryReader(new MemoryStream(packetData));
        var packetId = BinaryPrimitives.ReadInt16BigEndian(br2.ReadBytes(2));

        var content = new byte[packetData.Length - 2];
        Buffer.BlockCopy(packetData, 2, content, 0, packetData.Length - 2);

        var packets = new List<NetworkPacket>();
        
        if (reader.BaseStream.Length - 4 > packetLength)
        {
            var extra = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
            Buffer.BlockCopy(packet, (int)reader.BaseStream.Position, extra, 0, (int)(reader.BaseStream.Length - reader.BaseStream.Position));

            packets.AddRange(DecodePacketsFromBytes(extra));
        }
        
        packets.Add(new NetworkPacket(packetId, content));

        return packets;
    }
}