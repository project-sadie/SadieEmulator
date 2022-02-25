using Sadie.Game.Rooms.Users;
using Sadie.Networking.Writers;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserDataWriter : NetworkPacketWriter
{
    public RoomUserDataWriter(ICollection<RoomUser> users) : base(ServerPacketId.RoomUserData)
    {
        WriteInt(users.Count);

        foreach (var user in users)
        {
            WriteLong(user.Id);
            WriteString(user.Username);
            WriteString(user.Motto);
            WriteString(user.FigureCode);
            WriteLong(user.Id);
            WriteInt(user.Point.X);
            WriteInt(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInt(3);
            WriteInt(1);
            WriteString(user.Gender);
            WriteInt(-1);
            WriteInt(-1);
            WriteString("");
            WriteString("");
            WriteLong(user.AchievementScore);
            WriteBoolean(true);
        }
    }
}