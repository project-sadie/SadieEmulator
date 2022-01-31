namespace Sadie.Networking.Packets;

public interface INetworkPacketReader
{
    string ReadString();
    int ReadInt();
}