using Sadie.API.Game.Rooms.Users;
using Sadie.API.Networking;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserData)]
public class RoomUserDataWriter : AbstractPacketWriter
{
    public required ICollection<IRoomUser> Users { get; set; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Users))!, writer =>
        {
            Users = Users
                .Where(x => x.NetworkObject.Channel.IsWritable)
                .ToList();
            
            writer.WriteInteger(Users.Count);

            foreach (var user in Users)
            {
                writer.WriteLong(user.Player.Id);
                writer.WriteString(user.Player.Username);
                writer.WriteString(user.Player.AvatarData?.Motto ?? "");
                writer.WriteString(user.Player.AvatarData?.FigureCode ?? "");
                writer.WriteLong(user.Player.Id);
                writer.WriteInteger(user.Point.X);
                writer.WriteInteger(user.Point.Y);
                writer.WriteString(user.PointZ + "");
                writer.WriteInteger((int) user.Direction);
                writer.WriteInteger(1);
                writer.WriteString(user.Player.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
                writer.WriteInteger(-1);
                writer.WriteInteger(-1);
                writer.WriteString("");
                writer.WriteString("");
                writer.WriteInteger(user.Player.Data.AchievementScore);
                writer.WriteBool(true);
            }
        });
    }
}