using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsSaved)]
public class RoomSettingsSavedWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
}