using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

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
            WriteString(user.Player.Username);
            WriteString(user.Player.AvatarData.Motto);
            WriteString(user.Player.AvatarData.FigureCode);
            WriteInteger(user.Id);
            WriteInteger(user.Point.X);
            WriteInteger(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInteger(3);
            WriteInteger(1);
            WriteString(user.Player.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
            WriteInteger(-1);
            WriteInteger(-1);
            WriteString("");
            WriteString("");
            WriteInteger(user.Player.Data.AchievementScore);
            WriteBool(true);
        }
    }
}