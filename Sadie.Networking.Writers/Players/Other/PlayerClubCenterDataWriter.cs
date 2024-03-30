using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerClubCenterDataWriter : NetworkPacketWriter
{
    public PlayerClubCenterDataWriter(
        string joinDateString, 
        int streakInDays,
        double kickbackPercentage,
        int totalCreditsMissed,
        int totalCreditsRewarded,
        int totalCreditsSpent,
        int creditRewardForStreakBonus,
        int creditRewardForMonthlySpent,
        int timeUntilPayday)
    {
        WriteShort(ServerPacketId.HabboClubCenter);
        WriteInteger(streakInDays);
        WriteString(joinDateString);
        WriteString(kickbackPercentage.ToString());
        WriteInteger(totalCreditsMissed);
        WriteInteger(totalCreditsRewarded);
        WriteInteger(totalCreditsSpent);
        WriteInteger(creditRewardForStreakBonus);
        WriteInteger(creditRewardForMonthlySpent);
        WriteInteger(timeUntilPayday);
    }
}