using Sadie.Game.Rooms.Users;

namespace Sadie.Networking.Packets.Server.Rooms;

internal class RoomUserDataWriter : NetworkPacketWriter
{
    internal RoomUserDataWriter(ICollection<RoomUser> users) : base(ServerPacketId.RoomUserData)
    {
        WriteInt(users.Count);

        foreach (var user in users)
        {
            WriteLong(user.Id);
            WriteString("test");
            WriteString("sadie");
            WriteString("ea-3577-1408.ch-255-96.fa-1209-96.sh-290-96.hd-180-1.hr-828-61.ha-3763-1408.lg-280-89");
            WriteInt(1);
            WriteInt(user.Point.X);
            WriteInt(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInt((int) user.Direction);
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
}