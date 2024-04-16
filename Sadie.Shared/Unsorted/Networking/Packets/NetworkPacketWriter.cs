using System.Text;
using DotNetty.Buffers;

namespace Sadie.Shared.Unsorted.Networking.Packets;

public class NetworkPacketWriter
{
    private readonly IByteBuffer _buffer;
        
    protected NetworkPacketWriter()
    {
        _buffer = Unpooled.Buffer(6);
        _buffer.WriteInt(0);
    }

    protected void WriteString(string data)
    {
        WriteShort((short) data.Length);
        WriteBytes(Encoding.Default.GetBytes(data));
    }

    private void WriteBytes(byte[] data, bool reverse = false)
    {
        _buffer.WriteBytes(reverse ? data.Reverse().ToArray() : data);
    }

    protected void WriteShort(short data)
    {
        _buffer.WriteShort(data);
    }

    protected void WriteInteger(int data)
    {
        WriteBytes(BitConverter.GetBytes(data), true);
    }

    protected void WriteLong(long data)
    {
        WriteBytes(BitConverter.GetBytes((int)data), true);
    }

    protected void WriteBool(bool boolean)
    {
        WriteBytes(new[] {(byte) (boolean ? 1 : 0)});
    }

    public IByteBuffer GetAllBytes()
    {
        _buffer.SetInt(0, _buffer.WriterIndex - 4);
        return _buffer;
    }
}