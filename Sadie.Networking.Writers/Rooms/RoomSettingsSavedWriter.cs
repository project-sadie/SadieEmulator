using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsSaved)]
public class RoomSettingsSavedWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
}