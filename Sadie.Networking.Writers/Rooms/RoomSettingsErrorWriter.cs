using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsError)]
public class RoomSettingsErrorWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required int ErrorCode { get; init; }
    public required string Message { get; init; }
}