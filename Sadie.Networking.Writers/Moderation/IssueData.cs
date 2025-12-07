using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Moderation;

public class IssueData
{
    [PacketData] public required int IssueId { get; init; }
    [PacketData] public required int State { get; init; }
    [PacketData] public required int CategoryId { get; init; }
    [PacketData] public required int ReportedCategoryId { get; init; }
    [PacketData] public required int IssueAgeInMs { get; init; }
    [PacketData] public required int Priority { get; init; }
    [PacketData] public required int GroupingId { get; init; }
    [PacketData] public required int ReporterUserId { get; init; }
    [PacketData] public required string ReporterUsername { get; init; }
    [PacketData] public required int ReportedUserId { get; init; }
    [PacketData] public required string ReportedUsername { get; init; }
    [PacketData] public required int PickerUserId { get; init; }
    [PacketData] public required string PickerUsername { get; init; }
    [PacketData] public required string Message { get; init; }
    [PacketData] public required int ChatRecordId { get; init; }
    [PacketData] public required List<PatternMatchData> Patterns { get; init; }
}