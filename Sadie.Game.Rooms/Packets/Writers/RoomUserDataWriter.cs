using Sadie.Game.Rooms.Users;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomUserData)]
public class RoomUserDataWriter : AbstractPacketWriter
{
    public required ICollection<IRoomUser> Users { get; init; }

    public override void OnConfigureRules()
    {
        Override(PropertyHelper<RoomUserDataWriter>.GetProperty(x => x.Users), writer =>
        {
            writer.WriteInteger(Users.Count);

            foreach (var user in Users)
            {
                writer.WriteInteger(user.Id);
                writer.WriteString(user.Player.Username);
                writer.WriteString(user.Player.AvatarData.Motto);
                writer.WriteString(user.Player.AvatarData.FigureCode);
                writer.WriteInteger(user.Id);
                writer.WriteInteger(user.Point.X);
                writer.WriteInteger(user.Point.Y);
                writer.WriteString(user.PointZ + "");
                writer.WriteInteger((int) user.Direction);
                writer.WriteInteger(1); // type, 1 = user, 2 = pet, 3 = bot, 4 = rent bot
                writer.WriteString(user.Player.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
                writer.WriteInteger(-1); // group id
                writer.WriteInteger(-1); // group status
                writer.WriteString(""); // group name
                writer.WriteString(""); // swim figure
                writer.WriteInteger(user.Player.Data.AchievementScore);
                writer.WriteBool(true); // is moderator
            }
        });
    }
}