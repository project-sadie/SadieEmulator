using System.Buffers;
using System.Text;

namespace Sadie.Shared.Networking.Packets;

public class NetworkPacketWriter
{
    private readonly ArrayBufferWriter<byte> _packet;
        
    protected NetworkPacketWriter()
    {
        _packet = new ArrayBufferWriter<byte>();
    }

    protected void WriteString(string data)
    {
        WriteShort((short) data.Length);
        WriteBytes(Encoding.Default.GetBytes(data));
    }

    private void WriteBytes(byte[] data, bool reverse = false)
    {
        _packet.Write(reverse ? data.Reverse().ToArray() : data);
    }

    protected void WriteShort(short data)
    {
        WriteBytes(BitConverter.GetBytes(data), true);
    }

    protected void WriteInteger(int data)
    {
        WriteBytes(BitConverter.GetBytes(data), true);
    }

    protected void WriteLong(long data)
    {
        WriteBytes(BitConverter.GetBytes((int) data), true);
    }

    protected void WriteBool(bool boolean)
    {
        WriteBytes(new[] {(byte) (boolean ? 1 : 0)});
    }

    public byte[] GetAllBytes()
    {
        var bytes = new List<byte>();
            
        bytes.AddRange(BitConverter.GetBytes(_packet.WrittenCount));
        bytes.Reverse();
        bytes.AddRange(_packet.WrittenSpan.ToArray());

        return bytes.ToArray();
    }
}