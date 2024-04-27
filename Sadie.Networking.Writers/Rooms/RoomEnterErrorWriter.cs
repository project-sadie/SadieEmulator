using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

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