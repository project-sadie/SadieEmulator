using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomEnterError)]
public class RoomEnterErrorWriter : AbstractPacketWriter
{
    public required int ErrorCode { get; init; }

    public override void OnConfigureRules()
    {
        After(GetType().GetProperty(nameof(ErrorCode))!, writer =>
        {
            writer.WriteString("");
        });
    }
}