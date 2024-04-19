using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerSubscriptionWriter : NetworkPacketWriter
{
    public PlayerSubscriptionWriter(
        string name,
        int daysLeft,
        int memberPeriods,
        int periodsSubscribedAhead,
        int responseType,
        bool hasEverBeenMember,
        bool isVip,
        int pastClubDays,
        int pastVipDays,
        int minutesTillExpire,
        int minutesSinceModified)
    {
        WriteShort(ServerPacketId.PlayerSubscription);
        WriteString(name.ToLower());
        WriteInteger(daysLeft);
        WriteInteger(memberPeriods);
        WriteInteger(periodsSubscribedAhead);
        WriteInteger(responseType);
        WriteBool(hasEverBeenMember);
        WriteBool(isVip);
        WriteInteger(pastClubDays);
        WriteInteger(pastVipDays);
        WriteInteger(minutesTillExpire);
        WriteInteger(minutesSinceModified);
    }
}