using System.Buffers.Binary;
using System.Text;

namespace Sadie.Networking.Packets;

public class NetworkPacketReader : INetworkPacketReader
{
    private readonly byte[] _packetData;
    private int _packetPosition;
        
    protected NetworkPacketReader(byte[] packetData)
    {
        _packetData = packetData;
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

    public bool ReadBool()
    {
        return _packetData[_packetPosition++] == 1;
    }

    private byte[] ReadBytes(int bytes)
    {
        var data = new byte[bytes];

        for (var i = 0; i < bytes; i++)
        {
            data[i] = _packetData[_packetPosition++];
        }

        return data;
    }
}