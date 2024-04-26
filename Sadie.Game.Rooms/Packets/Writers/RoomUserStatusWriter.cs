using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomUserStatus)]
public class RoomUserStatusWriter : AbstractPacketWriter
{
    public required ICollection<IRoomUser> Users { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Users.Count);

        foreach (var user in Users)
        {
            var statusList = user.
                StatusMap.
                Select(x => x.Key + (string.IsNullOrEmpty(x.Value) ? "" : " " + x.Value));
            
            writer.WriteLong(user.Id);
            writer.WriteInteger(user.Point.X);
            writer.WriteInteger(user.Point.Y);
            writer.WriteString(user.PointZ + "");
            writer.WriteInteger((int) user.DirectionHead);
            writer.WriteInteger((int) user.Direction);
            writer.WriteString("/" + string.Join("/", statusList).TrimEnd('/'));
            
            user.NeedsStatusUpdate = false; 
        }
    }
}