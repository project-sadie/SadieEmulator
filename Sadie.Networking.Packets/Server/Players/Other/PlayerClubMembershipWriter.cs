namespace Sadie.Networking.Packets.Server.Players.Other;

internal class PlayerClubMembershipWriter : NetworkPacketWriter
{
    internal PlayerClubMembershipWriter(string subscription) : base(ServerPacketId.PlayerClubMembership)
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