using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomScore)]
public class RoomScoreWriter : AbstractPacketWriter
{
    public required int Score { get; init; }
    public required bool CanUpvote { get; init; }
}