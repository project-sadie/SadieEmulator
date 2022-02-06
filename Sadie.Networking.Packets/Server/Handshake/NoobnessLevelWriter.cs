namespace Sadie.Networking.Packets.Server.Handshake;

internal class NoobnessLevelWriter : NetworkPacketWriter
{
    internal NoobnessLevelWriter() : base(ServerPacketId.PlayerIdentity)
    {
        WriteInt(1);
    }
}