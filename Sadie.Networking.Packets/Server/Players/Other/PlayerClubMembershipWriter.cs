namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerClubMembershipWriter : NetworkPacketWriter
{
    public PlayerClubMembershipWriter(string subscription) : base(ServerPacketId.PlayerClubMembership)
    {
        // TODO: Pass structure in 
        
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