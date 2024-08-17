using Sadie.API.Game.Rooms.Users;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

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
            writer.WriteString(user.PointZ.ToString("0.00"));
            writer.WriteInteger((int) user.DirectionHead);
            writer.WriteInteger((int) user.Direction);
            writer.WriteString("/" + string.Join("/", statusList).TrimEnd('/'));
            
            user.NeedsStatusUpdate = false; 
        }
    }
}