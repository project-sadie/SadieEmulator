using Sadie.API.Interfaces.Game.Players.Packets.Writers;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Subscriptions;

[PacketId(ServerPacketId.PlayerSubscription)]
public class PlayerSubscriptionWriter : AbstractPacketWriter, IPlayerSubscriptionWriter
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