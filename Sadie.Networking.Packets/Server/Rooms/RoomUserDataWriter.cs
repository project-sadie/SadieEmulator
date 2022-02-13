using Sadie.Game.Players.Avatar;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomUserDataWriter : NetworkPacketWriter
{
    internal RoomUserDataWriter() : base(ServerPacketId.RoomUserData)
    {
        WriteInt(1);
        WriteInt(1);
        WriteString("test");
        WriteString("sadie");
        WriteString("ea-3577-1408.ch-255-96.fa-1209-96.sh-290-96.hd-180-1.hr-828-61.ha-3763-1408.lg-280-89");
        WriteInt(1);
        WriteInt(4); // x
        WriteInt(4); // y
        WriteString(0 + ""); // z
        WriteInt(0);
        WriteInt(1);
        WriteString("M");
        WriteInt(-1);
        WriteInt(-1);
        WriteString("");
        WriteString("");
        WriteInt(39214); // achievement score
        WriteBoolean(true);
    }
}