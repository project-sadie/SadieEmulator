namespace Sadie.Networking.Packets.Server.Players.Other;

internal class NoobnessLevelWriter : NetworkPacketWriter
{
    internal NoobnessLevelWriter() : base(ServerPacketId.PlayerIdentity)
    {
        WriteInt(1);
    }
}