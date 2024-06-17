using Sadie.Enums.Game.Rooms.Users.Trading;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeError)]
public class RoomUserTradeErrorWriter : AbstractPacketWriter
{
    public string Username { get; set; } = "";
    public required RoomUserTradeError Code { get; set; }

    public override void OnConfigureRules()
    {
        Override(GetType().GetProperty(nameof(Code))!, 
            writer => writer.WriteInteger((int)Code));
    }
}