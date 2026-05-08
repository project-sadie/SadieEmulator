using System.Buffers.Binary;
using System.Text;

namespace Sadie.Networking.Packets;

public ref struct NetworkPacketReader(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> _span = span;
    private int _position = 0;

    public int ReadInt()
        => BinaryPrimitives.ReadInt32BigEndian(Read(4));

    public long ReadLong()
        => BinaryPrimitives.ReadInt64BigEndian(Read(8));

    private short ReadShort()
        => BinaryPrimitives.ReadInt16BigEndian(Read(2));

    public bool ReadBool()
        => _span[_position++] == 1;

    public string ReadString()
    {
        int len = ReadShort();
        var slice = Read(len);
        return Encoding.UTF8.GetString(slice);
    }

    private ReadOnlySpan<byte> Read(int count)
    {
        var slice = _span.Slice(_position, count);
        _position += count;
        return slice;
    }
}
