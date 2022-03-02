using System.Collections.Generic;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Writers;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserStatusWriter : NetworkPacketWriter
{
    public RoomUserStatusWriter(ICollection<RoomUser> users) : base(ServerPacketId.RoomUserStatus)
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
            WriteString(user.StatusMap.Count < 1 ? "" : "/" + string.Join("", user.StatusMap.Select(x => x.Key + (string.IsNullOrEmpty(x.Value) ? "" : " " + x.Value))));
        }
    }
}