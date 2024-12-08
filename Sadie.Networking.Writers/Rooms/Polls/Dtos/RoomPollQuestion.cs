using Sadie.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Polls.Dtos;

public class RoomPollQuestion
{
    [PacketData] public required int Id { get; init; }
    [PacketData] public required int SortOrder { get; init; }
    [PacketData] public required int Type { get; init; }
    [PacketData] public required string Text { get; init; }
    [PacketData] public required int Category { get; init; }
    [PacketData] public required int AnswerType { get; init; }
    [PacketData] public required int AnswerCount { get; init; }
    public required List<RoomPollChoice> Choices { get; init; }
    [PacketData] public required List<RoomPollQuestion> Children { get; init; }
}