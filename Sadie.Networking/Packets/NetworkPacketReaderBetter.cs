using System.Buffers.Binary;
using System.Text;

namespace Sadie.Networking.Packets;

public class NetworkPacketReaderBetter : INetworkPacketReader
{
    private readonly byte[] _data;
    private int _position;

    public NetworkPacketReaderBetter(byte[] data)
    {
        _data = data;
    }
    
    public string ReadString()
    {
        int packetLength = BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));
        return Encoding.Default.GetString(ReadBytes(packetLength));
    }

    public int ReadInt()
    {
        return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
    }

    private byte[] ReadBytes(int bytes)
    {
        var data = new byte[bytes];

        for (var i = 0; i < bytes; i++)
        {
            data[i] = _data[_position++];
        }

        return data;
    }
}