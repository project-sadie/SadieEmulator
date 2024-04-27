using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsUpdated)]
public class RoomSettingsUpdatedWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
}