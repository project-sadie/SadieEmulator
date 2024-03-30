using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerClubCenterDataWriter : NetworkPacketWriter
{
    public PlayerClubCenterDataWriter(
        DateTime firstJoined, 
        int streakInDays,
        int kickbackPercentage,
        int totalCreditsMissed,
        int totalCreditsRewarded,
        int totalCreditsSpent,
        int creditRewardForStreakBonus,
        int creditRewardForMonthlySpent,
        int timeUntilPayday)
    {
        WriteShort(ServerPacketId.HabboClubCenter);
        WriteInteger(streakInDays);
        WriteString(firstJoined.ToString("dd/MM/yyyy"));
        WriteInteger(kickbackPercentage);
        WriteInteger(totalCreditsMissed);
        WriteInteger(totalCreditsRewarded);
        WriteInteger(totalCreditsSpent);
        WriteInteger(creditRewardForStreakBonus);
        WriteInteger(creditRewardForMonthlySpent);
        WriteInteger(timeUntilPayday);
    }
}