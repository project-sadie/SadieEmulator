using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettingsSaved)]
public class RoomSettingsSavedWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
}