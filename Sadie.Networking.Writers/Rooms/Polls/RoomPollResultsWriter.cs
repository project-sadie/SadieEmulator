using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Polls;

[PacketId(ServerPacketId.RoomPollResults)]
public class RoomPollResultsWriter : AbstractPacketWriter
{
    public required string Question { get; init; }
    public required Dictionary<string, int> Choices { get; init; }
    public required int Unknown { get; init; }
}