using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.HabboClubCenter)]
public class PlayerClubCenterDataWriter : AbstractPacketWriter
{
    public required int StreakInDays { get; init; }
    public required string JoinDateString { get; init; }
    public required string KickbackPercentageString { get; init; }
    public required int TotalCreditsMissed { get; init; }
    public required int TotalCreditsRewarded { get; init; }
    public required int TotalCreditsSpent { get; init; }
    public required int CreditRewardForStreakBonus { get; init; }
    public required int CreditRewardForMonthlySpent { get; init; }
    public required int TimeUntilPayday { get; init; }
}