using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserDance)]
public class RoomUserDanceWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int DanceId { get; init; }
}