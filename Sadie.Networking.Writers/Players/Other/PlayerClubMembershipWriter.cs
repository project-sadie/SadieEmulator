using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerClubMembershipWriter : NetworkPacketWriter
{
    public PlayerClubMembershipWriter(string subscription) : base(ServerPacketId.PlayerClubMembership)
    {
        WriteString(subscription);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteBoolean(false);
        WriteBoolean(false);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
    }
}