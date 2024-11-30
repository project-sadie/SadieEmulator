namespace Sadie.API;

public interface INetworkPacketWriter
{
    void WriteString(string data);
    void WriteShort(short data);
    void WriteInteger(int data);
    void WriteLong(long data);
    void WriteBool(bool boolean);
    byte[] GetAllBytes();
}