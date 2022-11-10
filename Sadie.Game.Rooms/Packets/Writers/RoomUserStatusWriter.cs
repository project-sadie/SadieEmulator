using Sadie.Game.Rooms.Users;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserStatusWriter : NetworkPacketWriter
{
    public RoomUserStatusWriter(ICollection<IRoomUser> users)
    {
        WriteShort(ServerPacketId.RoomUserStatus);
        WriteInteger(users.Count);

        foreach (var user in users)
        {
            var statusList = user.
                StatusMap.
                Select(x => x.Key + (string.IsNullOrEmpty(x.Value) ? "" : " " + x.Value));
            
            WriteLong(user.Id);
            WriteInteger(user.Point.X);
            WriteInteger(user.Point.Y);
            WriteString(user.Point.Z + "");
            WriteInteger((int) user.DirectionHead);
            WriteInteger((int) user.Direction);
            WriteString("/" + string.Join("", statusList));
        }
    }
}