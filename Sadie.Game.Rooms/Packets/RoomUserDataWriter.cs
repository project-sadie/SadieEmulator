using Sadie.Game.Rooms.Users;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserDataWriter : NetworkPacketWriter
{
    public RoomUserDataWriter(ICollection<IRoomUser> users)
    {
        WriteShort(ServerPacketId.RoomUserData);
        WriteInt(users.Count);

        foreach (var user in users)
        {
            WriteInt(user.Id);
            WriteString(user.AvatarData.Username);
            WriteString(user.AvatarData.Motto);
            WriteString(user.AvatarData.FigureCode);
            WriteInt(user.Id);
            WriteInt(user.Point.X);
            WriteInt(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInt(3);
            WriteInt(1);
            WriteString(user.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
            WriteInt(-1);
            WriteInt(-1);
            WriteString("");
            WriteString("");
            WriteLong(user.AvatarData.AchievementScore);
            WriteBoolean(true);
        }
    }
}