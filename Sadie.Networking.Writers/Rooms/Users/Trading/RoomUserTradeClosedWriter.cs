using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Rooms.Users.Trading;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeClosed)]
public class RoomUserTradeClosedWriter : AbstractPacketWriter
{
    public required long UserId { get; set; }
    public required RoomUserTradeCloseReason Reason { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Reason))!, 
            writer => writer.WriteInteger((int)Reason));
    }
}