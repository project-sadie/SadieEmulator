using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerSubscription)]
public class PlayerSubscriptionWriter : AbstractPacketWriter
{
    public required string Name { get; init; }
    public required int DaysLeft { get; init; }
    public required int MemberPeriods { get; init; }
    public required int PeriodsSubscribedAhead { get; init; }
    public required int ResponseType { get; init; }
    public required bool HasEverBeenMember { get; init; }
    public required bool IsVip { get; init; }
    public required int PastClubDays { get; init; }
    public required int PastVipDays { get; init; }
    public required int MinutesTillExpire { get; init; }
    public required int MinutesSinceModified { get; init; }
}