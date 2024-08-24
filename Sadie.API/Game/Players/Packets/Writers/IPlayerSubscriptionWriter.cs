namespace Sadie.API.Game.Players.Packets.Writers;

public interface IPlayerSubscriptionWriter
{
    string Name { get; init; }
    int DaysLeft { get; init; }
    int MemberPeriods { get; init; }
    int PeriodsSubscribedAhead { get; init; }
    int ResponseType { get; init; }
    bool HasEverBeenMember { get; init; }
    bool IsVip { get; init; }
    int PastClubDays { get; init; }
    int PastVipDays { get; init; }
    int MinutesTillExpire { get; init; }
    int MinutesSinceModified { get; init; }
}