using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets;

public class RoomUserStatusWriter : NetworkPacketWriter
{
    public RoomUserStatusWriter(ICollection<IRoomUser> users)
    {
        WriteShort(ServerPacketId.RoomUserStatus);
        WriteInt(users.Count);

        foreach (var user in users)
        {
            var statusList = user.
                StatusMap.
                Select(x => x.Key + (string.IsNullOrEmpty(x.Value) ? "" : " " + x.Value));
            
            WriteLong(user.Id);
            WriteInt(user.Point.X);
            WriteInt(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInt((int) user.DirectionHead);
            WriteInt((int) user.Direction);
            WriteString("/" + string.Join("", statusList));
        }
    }
}