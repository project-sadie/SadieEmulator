using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomScore)]
public class RoomScoreWriter : AbstractPacketWriter
{
    public required int Score { get; init; }
    public required bool CanUpvote { get; init; }
}