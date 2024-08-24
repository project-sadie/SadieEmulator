using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeStarted)]
public class RoomUserTradeStartedWriter : AbstractPacketWriter
{
    public required List<int> UserIds { get; init; }
    public required int State { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        foreach (var id in UserIds)
        {
            writer.WriteInteger(id);
            writer.WriteInteger(State);
        }
    }
}