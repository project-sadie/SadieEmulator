using Sadie.API.Networking;
using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Networking.Serialization.Attributes;

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