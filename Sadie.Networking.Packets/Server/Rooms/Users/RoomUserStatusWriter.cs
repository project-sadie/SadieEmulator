using Sadie.Game.Rooms.Users;

namespace Sadie.Networking.Packets.Server.Rooms.Users;

internal class RoomUserStatusWriter : NetworkPacketWriter
{
    internal RoomUserStatusWriter(ICollection<RoomUser> users) : base(ServerPacketId.RoomUserStatus)
    {
        WriteInt(users.Count);

        foreach (var user in users)
        {
            WriteLong(user.Id);
            WriteInt(user.Point.X);
            WriteInt(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInt((int) user.DirectionHead);
            WriteInt((int) user.Direction);
            WriteString(""); // statuses
        }
    }
}