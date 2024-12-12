using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Polls;

[PacketId(ServerPacketId.RoomPollResults)]
public class RoomPollResultsWriter : AbstractPacketWriter
{
    public required string Question { get; init; }
    public required Dictionary<string, int> Choices { get; init; }
    public required int Unknown { get; init; }
}