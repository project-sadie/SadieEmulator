using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeClosed)]
public class RoomUserTradeClosedWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required RoomUserTradeCloseReason Reason { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Reason))!, 
            writer => writer.WriteInteger((int)Reason));
    }
}