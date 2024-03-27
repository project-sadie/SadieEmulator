using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserDataWriter : NetworkPacketWriter
{
    public RoomUserDataWriter(ICollection<IRoomUser> users)
    {
        WriteShort(ServerPacketId.RoomUserData);
        WriteInteger(users.Count);

        foreach (var user in users)
        {
            WriteInteger(user.Id);
            WriteString(user.AvatarData.Username);
            WriteString(user.AvatarData.Motto);
            WriteString(user.AvatarData.FigureCode);
            WriteInteger(user.Id);
            WriteInteger(user.Point.X);
            WriteInteger(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInteger(3);
            WriteInteger(1);
            WriteString(user.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
            WriteInteger(-1);
            WriteInteger(-1);
            WriteString("");
            WriteString("");
            WriteInteger((int)user.AvatarData.AchievementScore);
            WriteBool(true);
        }
    }
}