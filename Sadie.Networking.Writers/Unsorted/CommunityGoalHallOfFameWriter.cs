using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Unsorted;

[PacketId(ServerPacketId.CommunityGoalHallOfFame)]
public class CommunityGoalHallOfFameWriter : AbstractPacketWriter
{
    public required string GoalCode { get; init; }
    public required List<HallOfFameEntryData> Data { get; init; }
}