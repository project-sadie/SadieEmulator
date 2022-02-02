namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerClub : NetworkPacketWriter
{
    public PlayerClub(string subscription) : base(ServerPacketId.PlayerClub)
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