namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerStatusWriter : NetworkPacketWriter
{
    internal PlayerStatusWriter(bool unknown1, bool unknown2, bool unknown3) : base(ServerPacketId.PlayerStatus)
    {
        WriteBoolean(unknown1);
        WriteBoolean(unknown2);
        WriteBoolean(unknown3);
    }
}