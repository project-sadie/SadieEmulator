namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerStatus : NetworkPacketWriter
{
    public PlayerStatus() : base(ServerPacketId.PlayerStatus)
    {
        WriteBoolean(true);
        WriteBoolean(false);
        WriteBoolean(true);
    }
}