using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsError)]
public class RoomSettingsErrorWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required int ErrorCode { get; init; }
    public required string Unknown { get; init; }
}