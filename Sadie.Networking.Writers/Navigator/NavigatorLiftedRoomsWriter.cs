using Sadie.API.Game.Rooms;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorLiftedRooms)]
public class NavigatorLiftedRoomsWriter : AbstractPacketWriter
{
    public required List<IRoomLogic> Rooms { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Rooms.Count);

        foreach (var room in Rooms)
        {
            writer.WriteLong(room.Id);
            writer.WriteInteger(0); // unknown
            writer.WriteString(""); // thumbnail?
            writer.WriteString(room.Name);
        }
    }
}