using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserData)]
public class RoomUserDataWriter : AbstractPacketWriter
{
    public required ICollection<IRoomUser> Users { get; set; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Users))!, writer =>
        {
            Users = Users
                .ToList();
            
            writer.WriteInteger(Users.Count);

            foreach (var user in Users)
            {
                writer.WriteLong(user.Player.Player.Id);
                writer.WriteString(user.Player.Player.Username);
                writer.WriteString(user.Player.Player.AvatarData?.Motto ?? "");
                writer.WriteString(user.Player.Player.AvatarData?.FigureCode ?? "");
                writer.WriteLong(user.Player.Player.Id);
                writer.WriteInteger(user.Point.X);
                writer.WriteInteger(user.Point.Y);
                writer.WriteString(user.PointZ + "");
                writer.WriteInteger((int) user.Direction);
                writer.WriteInteger(1);
                writer.WriteString(user.Player.Player.AvatarData.Gender == PlayerAvatarGender.Male ? "M" : "F");
                writer.WriteInteger(-1);
                writer.WriteInteger(-1);
                writer.WriteString("");
                writer.WriteString("");
                writer.WriteInteger(user.Player.Player.Data.AchievementScore);
                writer.WriteBool(true);
            }
        });
    }
}