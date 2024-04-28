using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsUpdated)]
public class RoomSettingsUpdatedWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
}