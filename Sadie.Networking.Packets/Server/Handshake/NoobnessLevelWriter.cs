namespace Sadie.Networking.Packets.Server.Handshake;

internal class NoobnessLevelWriter : NetworkPacketWriter
{
    internal NoobnessLevelWriter(int level) : base(ServerPacketId.NoobnessLevel)
    {
        WriteInt(level);
    }
}