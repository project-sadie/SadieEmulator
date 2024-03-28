using Sadie.Game.Players.Subscriptions;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerClubMembershipWriter : NetworkPacketWriter
{
    public PlayerClubMembershipWriter(string subscriptionName, IPlayerSubscription? subscription)
    {
        WriteShort(ServerPacketId.PlayerClubMembership);
        WriteString(subscriptionName);

        if (subscription == null)
        {
            WriteInteger(0);
            WriteInteger(7);
            WriteInteger(0);
            WriteInteger(1);
        }
        else
        {
            WriteInteger(10); // days left
            WriteInteger(2);
            WriteInteger(10); // months left
            WriteInteger(10); // years left
        }

        WriteBool(true); // unknown
        WriteBool(true); // unknown
        WriteInteger(0); // unknown
        WriteInteger(0); // unknown

        WriteInteger(subscription == null ? 0 : 10000);
    }
}