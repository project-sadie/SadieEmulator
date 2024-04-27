using Sadie.Networking.Serialization;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsErrorWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required int ErrorCode { get; init; }
    public required string Unknown { get; init; }
}