using System.Buffers;
using System.Text;
using Sadie.API;

namespace Sadie.Networking.Serialization;

public class NetworkPacketWriter : INetworkPacketWriter
{
    private readonly ArrayBufferWriter<byte> _packet = new();

    public void WriteString(string data)
    {
        WriteShort((short) data.Length);
        WriteBytes(Encoding.Default.GetBytes(data));
    }

    private void WriteBytes(byte[] data, bool reverse = false)
    {
        _packet.Write(reverse ? data.Reverse().ToArray() : data);
    }

    public void WriteShort(short data)
    {
        WriteBytes(BitConverter.GetBytes(data), true);
    }

    public void WriteInteger(int data)
    {
        WriteBytes(BitConverter.GetBytes(data), true);
    }

    public void WriteLong(long data)
    {
        WriteBytes(BitConverter.GetBytes((int)data), true);
    }

    public void WriteBool(bool boolean)
    {
        WriteBytes([(byte) (boolean ? 1 : 0)]);
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