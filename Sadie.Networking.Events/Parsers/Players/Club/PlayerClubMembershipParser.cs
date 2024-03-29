using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Parsers.Players.Club;

public class PlayerClubMembershipParser
{
    public string SubscriptionName { get; private set; }

    public void Parse(INetworkPacketReader reader)
    {
        SubscriptionName = reader.ReadString();
    }
}