using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Rooms.Users.Trading;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeError)]
public class RoomUserTradeErrorWriter : AbstractPacketWriter
{
    public string Username { get; set; } = "";
    public required RoomUserTradeError Code { get; init; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Code))!, 
            writer => writer.WriteInteger((int)Code));
    }
}