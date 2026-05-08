using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Polls.Dtos;

public class RoomPollChoice
{
    [PacketData] public required string Value { get; init; }
    [PacketData] public required string Text { get; init; }
    [PacketData] public required int Type { get; init; }
}