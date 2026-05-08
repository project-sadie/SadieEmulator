using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

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