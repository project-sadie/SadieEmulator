using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

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